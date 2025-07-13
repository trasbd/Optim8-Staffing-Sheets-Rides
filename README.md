# 🎢 Optim8 Staffing Sheets (Web Request Edition)

This WinForms tool generates staffing sheets from the [SixFlags.team](https://sixflags.team) scheduling portal, replacing the legacy Selenium-based version with a faster, more reliable system using direct HTTP requests.

## 🚀 Features

- 🔒 Secure login with "Remember Me" option (credentials encrypted per-user)
- 📅 Fetch schedules by **area and date** using `POST` requests
- 📍 Editable drag-and-drop **location ordering** (with optional file override via `SortOrder.txt`)
- 👷 Intelligent grouping of employees by **location and shift** (day/swing/night)
- 👶 Automatic highlighting of underage employees (yellow shading)
- 🚽 Optional **Restroom Merge Mode** (for Park Services only)
- 🧠 Location-based sort order and park-specific logic built-in
- 📄 Exports to a formatted Excel file with **group headers**, **bold restrooms**, and **shift grouping**
- 📂 Outputs saved in `GeneratedStaffingSheets/` folder with timestamped filenames

## 🧠 How It Works

1. **Login** with Company, Username, and Password
2. **Pick a date and area**
3. **Fetch Schedule** via `POST` to SixFlags.team
4. **Parse HTML** into `ScheduleEntry` objects (includes time parsing, tags, and shift detection)
5. **Sort entries** based on UI-reordered or file-based location order
6. **Export to Excel** using ClosedXML with intelligent formatting

## 📁 Folder Structure

```
O8SS-WebRequest/
├── Form1.cs               # Main UI and login logic
├── ScheduleService.cs     # Handles GET/POST web requests
├── ScheduleParser.cs      # Parses HTML responses into data objects
├── ScheduleEntry.cs       # Models schedule and employee data
├── ScheduleExporter.cs    # Outputs Excel staffing sheet
├── LocationOptionsForm.cs # Drag-and-drop location editor
├── SortOrder.txt          # (Optional) override location sort order
└── AboutForm.cs           # Shows build date version info
```

## 📦 Requirements

- .NET Framework 4.8
- [ClosedXML](https://github.com/ClosedXML/ClosedXML)
- [HtmlAgilityPack](https://html-agility-pack.net/)

## 🛠️ Setup & Usage

1. Open and build the solution in **Visual Studio 2019+**
2. Run the app and log in using your credentials
3. Select a date and area, then click **Go**
4. If desired, click **Options** to change the location order
5. The Excel file will auto-open and be saved to:

```
GeneratedStaffingSheets/yyyy-MM-dd_Area_YYYYMMDD-HHmmss.xlsx
```

## ✏️ Notes

- **Restroom grouping** mode (checkbox) pulls additional employee data and merges restrooms back into home locations.
- **Drag-and-drop ordering** is saved to `SortOrder.txt` for reuse.
- Park Services and Rides have separate built-in default location orders.

## 📜 License

MIT — see `LICENSE.md`

---

📬 Questions, issues, or feedback? Open an issue or contact the maintainer.
