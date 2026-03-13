# File & Database Monitoring Worker Service

A lightweight **.NET Worker Service** designed to monitor **file system events** and **database table changes** for data processing workflows.
The service continuously watches configured sources and processes newly detected data in near real-time.

This project demonstrates a **background service architecture commonly used in enterprise data ingestion systems**.

---

# Overview

Modern data platforms often require automated services that monitor incoming data sources and trigger processing workflows. This project implements two background workers:

* **File Monitoring Worker** – Detects newly created files in a configured directory.
* **Database Monitoring Worker** – Monitors selected database tables and processes newly inserted records.

Both workers run as **long-running background services** inside a .NET Worker application.

---

# Key Features

* Real-time **file system monitoring**
* Configurable **database table monitoring**
* Background processing using **.NET Worker Services**
* Configuration-driven design via `appsettings.json`
* Modular worker architecture
* Structured logging for observability

---

# Architecture

```id="k0l4nf"
                +-----------------------+
                |   Input File System   |
                |   C:\FileWatcher      |
                +-----------+-----------+
                            |
                            v
                   File Monitoring Worker
                            |
                            v
                    File Processing Logic
                            |
                            v
                       Target System


                +-----------------------+
                |   Source Database     |
                |   (Orders Table)     |
                +-----------+-----------+
                            |
                            v
                 Database Monitoring Worker
                            |
                            v
                  Data Processing Logic
                            |
                            v
                      Target Database
```

---

# Technology Stack

* .NET Worker Service
* C#
* SQL Server
* FileSystemWatcher API
* Dependency Injection
* JSON-based configuration
* Asynchronous background processing

---

# Project Structure

```id="noa3ls"
FileWatcherService
│
├── Program.cs
│
├── Workers
│   ├── Worker.cs
│   │     Handles file monitoring
│   │
│   └── DatabaseWorker.cs
│         Handles database monitoring
│
├── appsettings.json
│
└── README.md
```

---

# Configuration

The application behavior is controlled via `appsettings.json`.

Example configuration:

```json id="jsmucj"
{
  "TablesToMonitor": [
    "Orders"
  ]
}
```

This allows the worker service to dynamically monitor only selected tables.

---

# Database Setup

Example source table:

```sql id="4uxygt"
CREATE TABLE Orders
(
    Id INT PRIMARY KEY,
    CustomerName VARCHAR(100),
    Amount DECIMAL(10,2),
    Processed BIT DEFAULT 0
);
```

Target table used to track processed records:

```sql id="ab9cyg"
CREATE TABLE ProcessedData
(
    TableName VARCHAR(100),
    RowId INT,
    ProcessedTime DATETIME
);
```

---

# Sample Data

Insert test records:

```sql id="o86dpa"
INSERT INTO Orders (Id, CustomerName, Amount, Processed)
VALUES
(1, 'John Smith', 120.50, 0),
(2, 'Alice Johnson', 89.99, 0),
(3, 'Robert Brown', 250.75, 0);
```

Records with `Processed = 0` will be automatically picked up by the service.

---

# Running the Application

1. Clone the repository

```id="t7k9sv"
git clone https://github.com/your-username/windows-file-watcher-service.git
```

2. Navigate to the project directory

```id="m09w2a"
cd windows-file-watcher-service
```

3. Run the application

```id="5oluz9"
dotnet run
```

Once running, the service begins monitoring both configured sources.

---

# Example Log Output

```id="x98n06"
File watcher started at 2026-03-12 10:00:00

New file detected: C:\FileWatcher\Input\orders.txt

Database monitoring service started
Processing row 1 from table Orders
Processing row 2 from table Orders
```

---

# Use Cases

This architecture is commonly used in:

* Data ingestion pipelines
* ETL preprocessing systems
* File-based system integrations
* On-prem to cloud data synchronization
* Event-driven automation workflows

---

# Future Enhancements

Potential improvements include:

* Batch processing for database ingestion
* Parallel table monitoring
* Retry handling and error queues
* Structured logging and monitoring
* Windows Service deployment
* Integration with message brokers (Kafka / Service Bus)

---

# Author

**Manikumar Kotipalli**

---

# License

This project is intended for educational and demonstration purposes.
