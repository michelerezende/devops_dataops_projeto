import duckdb

# Conecta (ou cria) o banco de dados DuckDB
conn = duckdb.connect("erp.duckdb")

# Cria tabelas
conn.execute("""
CREATE TABLE IF NOT EXISTS clientes (
    id INTEGER PRIMARY KEY,
    nome TEXT,
    email TEXT
)
""")

conn.execute("""
CREATE TABLE IF NOT EXISTS produtos (
    id INTEGER PRIMARY KEY,
    nome TEXT,
    preco REAL,
    estoque INTEGER
)
""")

conn.execute("""
CREATE TABLE IF NOT EXISTS pedidos (
    id INTEGER PRIMARY KEY,
    cliente_id INTEGER,
    data TEXT,
    total REAL,
    FOREIGN KEY(cliente_id) REFERENCES clientes(id)
)
""")

conn.execute("""
CREATE TABLE IF NOT EXISTS itens_pedido (
    pedido_id INTEGER,
    produto_id INTEGER,
    quantidade INTEGER,
    preco_unit REAL,
    FOREIGN KEY(pedido_id) REFERENCES pedidos(id),
    FOREIGN KEY(produto_id) REFERENCES produtos(id)
)
""")

# Insere dados exemplo
conn.execute("INSERT INTO clientes VALUES (1, 'Maria Silva', 'maria@email.com') ON CONFLICT DO NOTHING")
conn.execute("INSERT INTO produtos VALUES (1, 'Notebook', 3500.00, 10), (2, 'Mouse', 80.00, 50) ON CONFLICT DO NOTHING")

print("Banco de dados criado com sucesso.")
conn.close()
