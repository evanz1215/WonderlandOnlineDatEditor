# WonderlandOnlineDatEditor

A WPF tool for viewing and editing Wonderland Online game data files.

Data path: `C:\Program Files (x86)\Chinesegamer\WLOnline\data`

## Game Data Files

以下為 `data` 資料夾中所有檔案及其用途：

| File | Size | Description |
|------|------|-------------|
| Item.Dat | 2.8 MB | 道具/裝備資料庫 — 6,425 筆道具記錄，含名稱、類型、圖示、屬性、價格、合成材料等 |
| Npc.dat | 648 KB | NPC/怪物資料庫 — 4,808 筆記錄，含名稱、等級、HP/SP、六維屬性、元素、技能、掉落物等 |
| Skill.dat | 129 KB | 技能資料庫 — 893 筆技能記錄，含名稱、SP消耗、攻擊力、元素屬性、技能範圍、前置技能等 |
| SceneData.dat | 147 KB | 場景/地圖資料 — 1,145 筆場景記錄，含場景ID、名稱、音效、控制旗標、特效設定等 |
| Compound2.dat | 51 KB | 合成配方資料庫 — 800 筆合成記錄，含成品ID、藍圖ID、工具、材料×5、數量×5、製作時間 |
| Compound.dat | 11 KB | 舊版合成配方 — 168 筆記錄，格式同 Compound2.dat |
| Ground.MMG | 21.9 MB | 地面碰撞/地形資料 — 1,164 張地圖的碰撞網格，每 20px 一格判定可行走區域 |
| Wem.MMG | 284 KB | 地圖物件/裝飾圖像 — 5,701 個節點，存放各場景的物件精靈圖資料 |
| Eve.emg | 4.9 MB | 地圖事件/世界資料 — 含 NPC 配置、傳送點、採礦區、物品生成點、戰鬥區域、事件觸發等 |
| Talk.dat | 4.9 MB | NPC 對話資料 — 17,661 筆固定 292 bytes 記錄，Big5 反轉存放，含 TalkID、文字長度、對話文本 |
| Mark.dat | 1.1 MB | 地圖標記資料 — 場景標記/圖標資訊（詳細格式未知） |
| SkillData.MBTM | 1.1 MB | 技能動畫資料 — 技能演出/動畫相關參數（詳細格式未知） |
| Formula.dat | 407 B | 公式/係數資料 — 浮點數陣列，可能為戰鬥或經驗值計算公式參數 |
| Gec.Dat | 56 B | 未知用途 — 小型加密資料（詳細格式未知） |
| Lbd.dat | 119 B | 未知用途 — 小型加密資料（詳細格式未知） |
| TrafficSetting.txt | 16 KB | 地圖傳送/交通設定 — 傳送路線ID、膠囊ID、兌換道具，UTF-8 文字格式 |

## Supported File Types (Editor)

### Dat Files — XOR Encrypted, Fixed-Record (完整解碼)

| File | Record Size | Records | XOR Keys (byte/word/dword) | Subtract |
|------|-------------|---------|---------------------------|----------|
| Item.Dat | 451 bytes | 6,425 | 0x9A / 0xEFC3 / 0x0B80F4B4 | 9 |
| Npc.dat | 138 bytes | 4,808 | 0xC8 / 0x5209 / 0x0BAEB716 | 9 |
| Skill.dat | 148 bytes | 893 | 0xFD / 0x6EA0 / 0x0BDEDEBF | 4 |
| SceneData.dat | 131 bytes | 1,145 | 0x2C / 0xEA6C / 0x062B7BA7 | 9 |
| Compound2.dat | 65 bytes | 800 | 0xD3 / 0xFBBC / 0x0A06F965 | 3 |

### MMG Files — Binary Archive, Index-at-End (完整解碼)

| File | Node Size | Description |
|------|-----------|-------------|
| Ground.MMG | 29 bytes | 地面碰撞/地形資料 (collision grid per map) |
| Wem.MMG | 22 bytes | 地圖物件/裝飾圖像資料 (map object sprites) |

### EMG Files — Multi-Section Binary (完整解碼)

| File | Description |
|------|-------------|
| Eve.emg | 地圖事件/世界資料 — 解析地圖摘要、NPC 配置、傳送點、進出口 |

### Text Files (完整解碼)

| File | Format | Description |
|------|--------|-------------|
| TrafficSetting.txt | UTF-8 CSV | 地圖傳送/交通設定 (traffic routes, capsule IDs, exchange items) |

### Talk.dat — NPC Dialogue (完整解碼)

| File | Record Size | Records | Description |
|------|-------------|---------|-------------|
| Talk.dat | 292 bytes | 17,661 | NPC 對話文本，Big5 反轉編碼，含 TalkID + 文字內容 |

### Mark.dat — Map Markers (完整解碼)

| File | Record Size | Records | Description |
|------|-------------|---------|-------------|
| Mark.dat | 553 bytes | 2,177 | 地圖標記/任務圖標資料，Big5 反轉編碼，含名稱 + 標籤 |

### Formula.dat — Game Coefficients (完整解碼)

| File | Description |
|------|-------------|
| Formula.dat | 公式/係數資料 — 47 個 IEEE 754 double 值（戰鬥/經驗值計算參數） |

### Small Dat Files — Structured Viewer (部分解碼)

| File | Records | Record Size | Description |
|------|---------|-------------|-------------|
| Gec.Dat | 3 | 18 bytes | 未知用途 — 每筆 9 個 uint16 值（XOR key 未知） |
| Lbd.dat | 3 | 39 bytes | 未知用途 — uint16+uint8 triplets（XOR key 未知） |

### Raw Hex Viewer (格式未知，以原始 Hex 檢視)

| File | Description |
|------|-------------|
| SkillData.MBTM | 技能動畫資料 — 以 Hex 顯示 |

> 未知格式的檔案會自動使用 Raw Hex Viewer 開啟，每行 16 bytes，顯示 Hex、ASCII、UInt16、UInt32 解碼值。

## Features

- Auto-detect file type by filename and extension
- View all records in a dark-themed DataGrid (Catppuccin)
- Search/filter across all columns
- Edit records inline (dat files)
- Save with automatic backup (.bak) (dat files)
- Export to CSV
- Big5 name decoding (reversed boundary / byte-swap methods)
- Big5 hex name decoding for TrafficSetting.txt
- Raw hex viewer for unknown binary formats
- Eve.emg full parser with map summary (NPC/Warp/Entry counts)

## Build

```bash
dotnet build
dotnet test
dotnet run --project src/WonderlandOnlineDatEditor
```

## Requirements

- .NET 8.0 SDK
- Windows (WPF)

## License

[MIT](LICENSE)
