## 4. General requirements

Can run both as a **standalone console application** and as a **Windows Service**.

## Configuration
The following parameters should be defined in a JSON file:
- [ ] ABORT_OVERDUE [true/false]: If true, aborts synchronisation if an overdue tasks is found in ToDo.
- [ ] REFRESH [10..60]: Refresh time.
- [ ] DEBUG [true/false]: If true, it logs DEBUG level messages.
- [ ] LOG [*text*]: Log file path.

## Logging

### Message levels
- - [ ]Messages should be assigned the following level:
- **INFO**: Essential to understand the progress.
- **WARNING**: Abormality that does not comprimise the execution.
- **ERROR**: Abormality that comprimises the execution.
- **DEBUG**: Debgug messages.

### Standalone console application
[ ] Messages should be saved in a `.txt` file.

### Windows Service
[ ] Messages should be save as events in the Windows Event Log.

