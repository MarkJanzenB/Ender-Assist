-- Schema version 2: G-code metadata columns on print_jobs
ALTER TABLE print_jobs ADD COLUMN estimated_duration_seconds INTEGER;
ALTER TABLE print_jobs ADD COLUMN filament_grams REAL;
ALTER TABLE print_jobs ADD COLUMN total_layers INTEGER;

INSERT INTO schema_version (version) VALUES (2);
