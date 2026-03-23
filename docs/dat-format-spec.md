# Wonderland Online .Dat 檔案格式解碼規格書

## 概述

Wonderland Online 的遊戲資料檔案 (.dat) 使用 XOR 加密和自訂的名稱儲存格式。本文檔記錄了各種 .dat 檔案的完整解碼流程。

---

## 1. XOR 加密機制

### 公式

所有數值欄位（byte, word, dword）使用相同的 XOR 加密模式：

```
解碼: decoded = (raw_value ^ xor_key) - subtract
編碼: encoded = (value + subtract) ^ xor_key
```

### 各檔案的 XOR 金鑰

| 檔案 | Byte Key | Word Key | DWord Key | Subtract |
|------|----------|----------|-----------|----------|
| Item.Dat | `0x9A` | `0xEFC3` | `0x0B80F4B4` | 9 |
| Npc.dat | `0xC8` | `0x5209` | `0x0BAEB716` | 9 |
| Skill.dat | `0xFD` | `0x6EA0` | `0x0BDEDEBF` | 4 |
| SceneData.dat | `0x2C` | `0xEA6C` | `0x062B7BA7` | 9 |
| Compound2.dat | `0xD3` | `0xFBBC` | `0x0A06F965` | 3 |

### Item.Dat 特殊 Stat XOR

Item 的 StatVal 和 UnknownVal 欄位使用特殊金鑰：
```
Word Key: 0xF4B4
Subtract: 109
```

---

## 2. 名稱編碼格式

名稱使用 **Big5 編碼**，儲存在 21 bytes 的欄位中：`[length 1B][name_data 20B]`。

### 2.1 反轉存儲法（Item / NPC）

名稱的 Big5 bytes 以**反轉順序**存放在 20 byte 陣列的特定區域中。

**解碼公式：**
```
nameBytes[i] = data[offset + 1 + (boundary - 1 - i)]   // i = 0..len-1
```

**邊界值：**
- **Item.Dat**: `boundary = 14`（名稱存放在 index 0~13，靠右對齊到 index 13）
- **Npc.dat**: `boundary = 10`（名稱存放在 index 0~9，靠右對齊到 index 9）

**最大名稱長度：**
- Item: 14 bytes（7 個中文字）
- NPC: 10 bytes（5 個中文字）

**範例 — Item 名稱「餐盤」(len=4)：**
```
20-byte 陣列: 00 00 00 00 00 00 00 00 00 00 [4C BD 5C C0] 90 D8 C8 AA EB 32
                                               ^^^^^^^^^^^^^^
                                               index 10-13 (boundary=14, 靠右)
讀取: data[13]=C0, data[12]=5C, data[11]=BD, data[10]=4C
反轉後: C0 5C BD 4C → Big5 解碼 → 「餐盤」
```

**範例 — NPC 名稱「波斯貓」(len=6)：**
```
20-byte 陣列: 00 00 00 00 [DF BF B5 B4 69 AA] CA F0 78 05 56 75 49 0B 18 D3
                           ^^^^^^^^^^^^^^^^^^^^
                           index 4-9 (boundary=10, 靠右)
讀取: data[9]=AA, data[8]=69, data[7]=B4, data[6]=B5, data[5]=BF, data[4]=DF
反轉後: AA 69 B4 B5 BF DF → Big5 解碼 → 「波斯貓」
```

### 2.2 Byte-Swap 法（Skill / Scene）

技能和場景名稱使用**對稱交換**（swap [i] with [19-i]），等同於完整的 byte 反轉。

**解碼步驟：**
1. 複製 20-byte 名稱資料
2. 對每個 i = 0..9，交換 `tmp[i]` 和 `tmp[19-i]`
3. 取前 `len` bytes
4. 以 Big5 解碼

```csharp
byte[] tmp = new byte[20];
Array.Copy(data, offset + 1, tmp, 0, 20);
for (int i = 0; i < 10; i++)
    (tmp[i], tmp[19 - i]) = (tmp[19 - i], tmp[i]);
// 取 tmp[0..len-1] 做 Big5 解碼
```

**範例 — Skill 名稱「防禦」(len=4)：**
```
原始 20 bytes: [XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX 禦 防 00 00]
Swap 後:       [00 00 防 禦 XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX]
取前 4 bytes → Big5 解碼 → 「防禦」
```

### 2.3 技能描述

技能描述使用相同的 Byte-Swap 法，但在 31-byte 欄位中 `[length 1B][desc_data 30B]`：
```csharp
for (int i = 0; i < 15; i++)
    (tmp[i], tmp[29 - i]) = (tmp[29 - i], tmp[i]);
```

---

## 3. Item 描述

Item 描述佔 255 bytes `[length 1B][desc_data 254B]`，使用反轉存儲法（boundary=254）。
但在實際資料中，所有 6425 筆 Item 的描述長度均為 0。

---

## 4. 記錄格式

### 4.1 Item.Dat — 451 bytes/record, 共 6425 筆

| Offset | Size | Field | XOR |
|--------|------|-------|-----|
| 0 | 21 | Name (反轉, boundary=14) | 無 |
| 21 | 1 | ItemType | byte |
| 22 | 2 | ItemID | word |
| 24 | 2 | IconNum | word |
| 26 | 2 | LargeIconNum | word |
| 28 | 8 | EquipImageNum[4] | word×4 |
| 36 | 4 | StatType[2] | word×2 |
| 40 | 1 | UnknownByte1 | byte |
| 41 | 1 | UnknownByte2 | byte |
| 42 | 8 | StatVal/UnknownVal (特殊XOR) | word×4 |
| 50 | 4 | Bytes (Rank, EquipPos, etc.) | byte×4 |
| 54 | 64 | ColorDef[16] | dword×16 |
| 118 | 2 | Unused, Level | byte×2 |
| 120 | 4 | BuyingPrice | dword |
| 124 | 4 | SellingPrice | dword |
| 128 | 1 | EquipLimit | byte |
| 129 | 2 | Control | word |
| 131 | 4 | UnknownDWord1 | dword |
| 135 | 1 | SetID | byte |
| 136 | 4 | AntiSeal | dword |
| 140 | 2 | SkillID | word |
| 142 | 10 | MaterialTypes[5] | word×5 |
| 152 | 255 | Description (反轉, boundary=254) | 無 |
| 407 | 3 | TentWidth/Height/Depth | byte×3 |
| 410 | 2 | UnknownWord2 | word |
| 412 | 3 | InvWidth/Height/UnknownByte5 | byte×3 |
| 415 | 4 | InTentImages[2] | word×2 |
| 419 | 2 | NpcID | word |
| 421 | 6 | UnknownBytes 6-11 | byte×6 |
| 427 | 2 | Duration | word |
| 429 | 2 | UnknownWord4 | word |
| 431 | 2 | CapsuleForm | word |
| 433 | 4 | UnknownWord6/7 | word×2 |
| 437 | 12 | UnknownDWord2/3/4 | dword×3 |
| 449 | 2 | UnknownWord8 | word |

### 4.2 Npc.dat — 138 bytes/record, 共 4808 筆

| Offset | Size | Field | XOR |
|--------|------|-------|-----|
| 0 | 21 | Name (反轉, boundary=10) | 無 |
| 21 | 1 | Type | byte |
| 22 | 2 | NpcID | word |
| 24 | 2 | ImageNum | word |
| 26 | 2 | ImageNumSmall | word |
| 28 | 16 | ColorCode1-4 | dword×4 |
| 44 | 4 | Catchable + Unknowns + Level | byte×4 |
| 48 | 8 | HP, SP | dword×2 |
| 56 | 10 | STR/CON/INT/WIS/AGI | word×5 |
| 66 | 2 | ImageNumEnlarge, Element | byte×2 |
| 68 | 6 | SkillIDs[3] | word×3 |
| 74 | 10 | DropItemIDs[5] | word×5 |
| 84 | ... | (其餘欄位見原始碼) | ... |
| 128 | 8 | UnknownDword2/3 | dword×2 |
| 136 | 2 | UnknownWord30 | word |

### 4.3 Skill.dat — 148 bytes/record, 共 893 筆

| Offset | Size | Field | XOR |
|--------|------|-------|-----|
| 0 | 21 | Name (Byte-Swap) | 無 |
| 21 | 1 | Type | byte |
| 22 | 2 | SkillID | word |
| 24 | 2 | SP | word |
| 26 | 1 | ElementType | byte |
| 27 | 2 | Attack | word |
| 29 | ... | (其餘欄位見原始碼) | ... |
| 117 | 31 | Description (Byte-Swap, 30B) | 無 |

### 4.4 SceneData.dat — 131 bytes/record, 共 1145 筆

| Offset | Size | Field | XOR |
|--------|------|-------|-----|
| 0 | 2 | SceneID | word |
| 2 | 21 | Name (Byte-Swap) | 無 |
| 23 | 2 | UnknownByte1/2 | byte×2 |
| 25 | 8 | SoundMediaName (1B len + 7B) | 無 |
| 33 | ... | Control, Effects, etc. | ... |

### 4.5 Compound2.dat — 65 bytes/record, 共 800 筆

無名稱欄位，全部為數值欄位。

---

## 5. 位元組序

所有多位元組數值使用 **Little-Endian** 格式：
```
UInt16: data[offset] | (data[offset+1] << 8)
UInt32: data[offset] | (data[offset+1] << 8) | (data[offset+2] << 16) | (data[offset+3] << 24)
```

---

## 6. 名稱解碼方法對照表

| 檔案 | 方法 | 邊界/參數 | 最大長度 |
|------|------|-----------|----------|
| Item.Dat | 反轉存儲 | boundary=14 | 14 bytes |
| Npc.dat | 反轉存儲 | boundary=10 | 10 bytes |
| Skill.dat | Byte-Swap | swap [i]↔[19-i] | 20 bytes |
| SceneData.dat | Byte-Swap | swap [i]↔[19-i] | 20 bytes |
| Compound2.dat | N/A | 無名稱欄位 | N/A |
