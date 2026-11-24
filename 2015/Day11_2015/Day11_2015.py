from pathlib import Path

def solve(password: str):
    letters = "abcdefghijklmnopqrstuvwxyz"
    idx_map = {c: i for i, c in enumerate(letters)}
    forbidden = {idx_map["i"], idx_map["o"], idx_map["l"]}

    def inc(nums):
        i = len(nums) - 1
        while i >= 0:
            nums[i] = (nums[i] + 1) % 26
            if nums[i] != 0:
                break
            i -= 1
        return nums

    def valid(nums):
        if any(x in forbidden for x in nums):
            return False
        straight = any(
            nums[i + 1] == nums[i] + 1 and nums[i + 2] == nums[i] + 2
            for i in range(len(nums) - 2)
        )
        if not straight:
            return False
        pairs = set()
        i = 0
        while i < len(nums) - 1:
            if nums[i] == nums[i + 1]:
                pairs.add(nums[i])
                i += 2
            else:
                i += 1
        return len(pairs) >= 2

    def next_pass(nums):
        nums = inc(nums[:])
        while not valid(nums):
            nums = inc(nums)
        return nums

    cur = [idx_map[c] for c in password.strip()]
    p1_nums = next_pass(cur)
    p2_nums = next_pass(p1_nums)
    return "".join(letters[x] for x in p1_nums), "".join(letters[x] for x in p2_nums)

if __name__ == "__main__":
    raw = Path("input_11.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Día 11 -> Parte 1: {p1} | Parte 2: {p2}")
