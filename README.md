# WonderlandOnlineDatEditor

A WPF tool for viewing and editing Wonderland Online `.dat` game data files.

## Supported File Types

| File | Record Size | XOR Keys (byte/word/dword) | Subtract |
|------|-------------|---------------------------|----------|
| Item.Dat | 451 bytes | 0x9A / 0xEFC3 / 0x0B80F4B4 | 9 |
| Npc.dat | 138 bytes | 0xC8 / 0x5209 / 0x0BAEB716 | 9 |
| Skill.dat | 148 bytes | 0xFD / 0x6EA0 / 0x0BDEDEBF | 4 |
| SceneData.dat | 131 bytes | 0x2C / 0xEA6C / 0x062B7BA7 | 9 |
| Compound2.dat | 65 bytes | 0xD3 / 0xFBBC / 0x0A06F965 | 3 |

## Features

- Auto-detect file type by filename
- View all records in a dark-themed DataGrid
- Search/filter across all columns
- Edit records inline
- Save with automatic backup (.bak)
- Export to CSV

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
