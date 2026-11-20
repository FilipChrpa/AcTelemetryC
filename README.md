# Assetto Corsa Real-Time Telemetry (AcTelemetryC)

A C# / .NET desktop application for reading, processing, and visualizing real-time vehicle telemetry data from **Assetto Corsa** via Windows Shared Memory (`MemoryMappedFile`).

## Key Features
- **Shared Memory Physics Reader (`AcTelemetryC`):** Dedicated data structures and low-latency reader parsing live physics data (speed, RPM, gear, throttle/brake inputs, etc.) directly from the simulator's memory-mapped files.
- **Live Telemetry Dashboard (`AcTelemetryUI`):** WPF user interface displaying real-time telemetry metrics and live pedal input graphs powered by **ScottPlot 5**.

## Tech Stack
- **Language:** C# (.NET)
- **UI Framework:** WPF (Windows Presentation Foundation)
- **Data Visualization:** ScottPlot 5
- **IPC / Data Source:** Windows Memory-Mapped Files (Shared Memory API)

## Project Structure
- `AcTelemetryC/` – Core library containing physics data structs and the Shared Memory reader.
- `AcTelemetryUI/` – WPF desktop application with real-time charts and dashboard components.

## Future Roadmap
- [ ] Lap-by-lap telemetry recording and export (JSON/CSV)
- [ ] Local database storage (SQL) for historical session comparison
- [ ] AI Race Engineer integration via REST API to analyze braking points and lap performance
