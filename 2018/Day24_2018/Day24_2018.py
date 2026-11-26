from dataclasses import dataclass
from pathlib import Path
import re

@dataclass
class Group:
  gid: int
  army: str
  units: int
  hp: int
  atk: int
  atk_type: str
  init: int
  weak: set[str]
  immune: set[str]

  @property
  def power(self) -> int:
    return self.units * self.atk

def parse_groups(text: str) -> list[Group]:
  blocks = text.strip().split("\n\n")
  groups: list[Group] = []
  gid = 1

  for army, block in [("immune", blocks[0]), ("infection", blocks[1])]:
    for line in block.splitlines()[1:]:
      line = line.strip()
      if not line:
        continue
      m = re.match(
          r"(\d+) units each with (\d+) hit points (?:\(([^)]+)\) )?with an"
          r" attack that does (\d+) (\w+) damage at initiative (\d+)",
          line,
      )
      units, hp, mods, atk, atk_type, init = m.groups()
      weak: set[str] = set()
      immune: set[str] = set()
      if mods:
        for part in mods.split(";"):
          part = part.strip()
          if part.startswith("weak to "):
            weak.update(x.strip() for x in part[8:].split(","))
          elif part.startswith("immune to "):
            immune.update(x.strip() for x in part[10:].split(","))

      groups.append(
          Group(
              gid=gid,
              army=army,
              units=int(units),
              hp=int(hp),
              atk=int(atk),
              atk_type=atk_type,
              init=int(init),
              weak=weak,
              immune=immune,
          )
      )
      gid += 1
  return groups

def damage_dealt(attacker: Group, defender: Group) -> int:
  if attacker.atk_type in defender.immune:
    return 0
  dmg = attacker.power
  if attacker.atk_type in defender.weak:
    dmg *= 2
  return dmg


def run_battle(base_groups: list[Group], boost: int = 0) -> tuple[str, int]:
  groups = [
      Group(
          gid=g.gid,
          army=g.army,
          units=g.units,
          hp=g.hp,
          atk=g.atk + (boost if g.army == "immune" else 0),
          atk_type=g.atk_type,
          init=g.init,
          weak=set(g.weak),
          immune=set(g.immune),
      )
      for g in base_groups
  ]

  while True:
    alive = [g for g in groups if g.units > 0]
    armies = {g.army for g in alive}
    if len(armies) == 1:
      winner = next(iter(armies))
      return winner, sum(g.units for g in alive)

    order_select = sorted(alive, key=lambda g: (-g.power, -g.init))
    chosen_ids: set[int] = set()
    targets: dict[int, Group] = {}

    for attacker in order_select:
      candidates = [
          d
          for d in alive
          if d.army != attacker.army
          and d.gid not in chosen_ids
          and damage_dealt(attacker, d) > 0
      ]
      if candidates:
        target = max(
            candidates,
            key=lambda d: (damage_dealt(attacker, d), d.power, d.init),
        )
        chosen_ids.add(target.gid)
        targets[attacker.gid] = target

    total_kills = 0
    for attacker in sorted(alive, key=lambda g: -g.init):
      if attacker.units <= 0 or attacker.gid not in targets:
        continue
      defender = targets[attacker.gid]
      if defender.units <= 0:
        continue
      dmg = damage_dealt(attacker, defender)
      kills = min(defender.units, dmg // defender.hp)
      defender.units -= kills
      total_kills += kills

    if total_kills == 0:
      return "stalemate", 0

def solve(text: str) -> tuple[int, int]:
  base = parse_groups(text)
  _, part1 = run_battle(base, boost=0)

  boost = 1
  while True:
    winner, rem = run_battle(base, boost=boost)
    if winner == "immune":
      return part1, rem
    boost += 1

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 24 -> Part 1: {p1} | Part 2: {p2}")