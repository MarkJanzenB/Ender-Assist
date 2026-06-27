-- Initial schema (version 1)
CREATE TABLE IF NOT EXISTS schema_version (
    version INTEGER NOT NULL PRIMARY KEY
);

CREATE TABLE IF NOT EXISTS print_jobs (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    file_path TEXT NOT NULL,
    name TEXT NOT NULL,
    status TEXT NOT NULL,
    priority INTEGER NOT NULL,
    progress REAL NOT NULL DEFAULT 0,
    total_lines INTEGER NOT NULL DEFAULT 0,
    last_line_sent INTEGER NOT NULL DEFAULT 0,
    queue_order INTEGER NOT NULL DEFAULT 0,
    created_at TEXT NOT NULL,
    started_at TEXT,
    completed_at TEXT,
    error_message TEXT
);

CREATE INDEX IF NOT EXISTS idx_print_jobs_status ON print_jobs(status);
CREATE INDEX IF NOT EXISTS idx_print_jobs_queue ON print_jobs(status, priority DESC, queue_order ASC);

CREATE TABLE IF NOT EXISTS printer_profiles (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    port TEXT NOT NULL DEFAULT '',
    baud_rate INTEGER NOT NULL DEFAULT 115200,
    buffer_size INTEGER NOT NULL DEFAULT 4,
    is_default INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS settings (
    key TEXT PRIMARY KEY,
    value TEXT NOT NULL
);

INSERT OR IGNORE INTO schema_version (version) VALUES (1);

INSERT OR IGNORE INTO printer_profiles (id, name, port, baud_rate, buffer_size, is_default)
VALUES (1, 'Ender 3 V2', '', 115200, 4, 1);
