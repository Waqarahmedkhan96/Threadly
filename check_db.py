import os
import sqlite3
paths=['Server/WebApi/app.db','Server/EfcRepositories/app.db']
for p in paths:
    print('PATH', p, 'EXISTS', os.path.exists(p), 'SIZE', os.path.getsize(p) if os.path.exists(p) else 'N/A')
    if os.path.exists(p):
        try:
            conn = sqlite3.connect(p)
            cur = conn.cursor()
            cur.execute("SELECT name FROM sqlite_master WHERE type='table';")
            tables = [r[0] for r in cur.fetchall()]
            print('TABLES', tables)
            for t in ['Users','Posts','Comments']:
                if t in tables:
                    cur.execute(f'SELECT count(*) FROM {t}')
                    print(t, 'count', cur.fetchone()[0])
            conn.close()
        except Exception as e:
            print('ERROR', e)
