# YourOwnJournal Documentation

## Project Structure
- `Models/` Entities and view models
- `Data/` EF Core DbContext + initializer
- `Repositories/` CRUD data access
- `Services/` Business logic, analytics, export, security
- `Components/Layout/` Blazor layout + navigation
- `Components/Pages/` Feature pages
- `wwwroot/` Bootstrap + custom CSS
- `Docs/` Schema and coursework evidence

## SQLite Schema
See `Docs/schema.sql` for the full schema. EF Core uses the same structure:
- `JournalEntries` with unique `EntryDate`
- `Moods` and `EntrySecondaryMoods`
- `Tags` and `EntryTags`
- `AppSettings` for theme + page size
- `AppLocks` for PIN hash + salt

## Seeding Strategy
Seeded in `Data/AppDbContext.cs`:
- Prebuilt moods with Positive/Neutral/Negative categories
- Prebuilt tags (Work, Health, Family, Travel, Learning, Reflection)

## Architecture Summary
- `Repositories` handle EF Core queries.
- `Services` enforce business rules (one entry per day, max 2 secondary moods, timestamps, PIN hashing, streaks, analytics, export).
- UI pages are Blazor components using Bootstrap 5 and custom CSS.

## Feature Coverage Checklist
1. Journal entry CRUD (one per day) + timestamps
2. Markdown editor with preview
3. Mood tracking (primary + up to 2 secondary)
4. Tags (prebuilt + custom)
5. Calendar navigation (month view)
6. Paginated entries list
7. Search + filters (date, mood, tag)
8. Streak tracking + missed days
9. Theme stored in DB
10. Dashboard analytics + date range
11. PIN security (hash + salt)
12. PDF export for date range

## How To Run
1. Open the solution in Visual Studio 2022+ (MAUI workload installed).
2. Restore packages and build.
3. Run on Windows, macOS, or Android target.

CLI:
```bash
cd /Users/nishantiwari/Desktop/appdev2/YourOwnJournal
dotnet restore
dotnet build
```

## Testing Notes
Manual test cases (minimum evidence):
- Create a journal entry today; verify timestamps and one-per-day constraint.
- Attempt to create a second entry on the same day; ensure validation error.
- Apply primary + secondary moods; ensure max 2 secondary enforced.
- Add custom tag + prebuilt tags; verify saved and reloaded.
- Use calendar to open a date and edit entry.
- Search by title/content; filter by date range + mood + tags.
- Verify streak counts by creating consecutive entries and checking streaks page.
- Toggle theme and verify persistence on restart.
- Set a PIN, restart app, verify lock screen.
- Export PDF for a range and open output file.

## Wireframe Notes
See `Docs/wireframes.md` for layout sketches.
