# 🎢 Optim8 Staffing Sheets (Web Request Edition)

This project automates the generation of staffing sheets from the SixFlags.team scheduling portal, replacing the old Selenium-based tool with a faster and more reliable approach using direct HTTP requests.

## 🚀 Features

- 🔒 Login directly to scheduling site
- 📅 Fetch schedule data by date and area
- 📍 Load and customize location sort order
- 📄 Export formatted Excel staffing sheets (ClosedXML)
- 📦 Self-contained WinForms UI with drag-and-drop location sorting

<!--
## 🖼️ Screenshots

*(Add screenshots of the main form, location editor, and sample Excel output here)*
-->
## 🧠 How It Works

1. **Login** using your Company ID, Username, and Password
2. **Select an Area** and Date
3. **Fetch Schedule** data via `POST` request (no browser automation)
4. **Parse HTML** into structured `ScheduleEntry` objects
5. **Sort Entries** by preferred location order (`SortOrder.txt`)
6. **Export to Excel**, with support for:
   - Day/Swing/Night shifts
   - Age tags (yellow shading)

## 📁 Folder Structure

```
O8SS-WebRequest/
├── Form1.cs               # Main UI logic and login workflow
├── ScheduleService.cs     # Handles GET/POST to sixflags.team
├── ScheduleParser.cs      # Converts HTML to structured data
├── ScheduleExporter.cs    # Generates Excel using ClosedXML
├── ScheduleEntry.cs       # Models a schedule row and employee info
├── LocationOptionsForm.cs # Lets user drag-and-drop location order
├── SortOrder.txt          # User-defined location ordering
└── ...
```

## ⚙️ Requirements

- .NET Framework 4.8
- [ClosedXML](https://github.com/ClosedXML/ClosedXML) (for Excel export)
- [HtmlAgilityPack](https://html-agility-pack.net/) (for HTML parsing)

## 🛠️ Setup & Usage

1. **Build the solution** in Visual Studio 2019 or newer.
2. Place `SortOrder.txt` next to the executable to define your preferred location order.
3. Run the app, log in, select an area + date, and hit **Go**.

✅ The Excel file will be saved to a `GeneratedStaffingSheets` folder next to the `.exe`.

## ✏️ Notes

- Location order is saved and shared across all users via `SortOrder.txt`.
- Locations can be reordered via drag-and-drop using the **Options** window.

## 📜 License

MIT — see `LICENSE.md`

---

📬 Questions, issues, or feedback? Feel free to open an issue!
