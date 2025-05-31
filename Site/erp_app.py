import streamlit as st
import duckdb
import pandas as pd
from datetime import datetime

# Conectar ao banco DuckDB
conn = duckdb.connect("erp.duckdb")

st.set_page_config(page_title="Mini ERP", layout="wide")
st.title("💼 Sistema ERP - Didático")

# Abas principais
aba = st.sidebar.radio("Menu", ["📋 Clientes", "📦 Produtos", "🧾 Pedidos"])

if aba == "📋 Clientes":
    st.header("Clientes")
    clientes = conn.execute("SELECT * FROM clientes").df()
    st.dataframe(clientes)

    with st.form("novo_cliente"):
        st.subheader("Adicionar Cliente")
        nome = st.text_input("Nome")
        email = st.text_input("Email")
        submitted = st.form_submit_button("Adicionar")
        if submitted:
            conn.execute("INSERT INTO clientes VALUES ((SELECT COALESCE(MAX(id), 0) + 1 FROM clientes), ?, ?) ", (nome, email))
            st.success("Cliente adicionado!")

elif aba == "📦 Produtos":
    st.header("Produtos")
    produtos = conn.execute("SELECT * FROM produtos").df()
    st.dataframe(produtos)

    with st.form("novo_produto"):
        st.subheader("Adicionar Produto")
        nome = st.text_input("Nome do Produto")
        preco = st.number_input("Preço", min_value=0.0, step=0.01)
        estoque = st.number_input("Estoque", min_value=0, step=1)
        submitted = st.form_submit_button("Adicionar")
        if submitted:
            conn.execute("INSERT INTO produtos VALUES ((SELECT COALESCE(MAX(id), 0) + 1 FROM produtos), ?, ?, ?)", (nome, preco, estoque))
            st.success("Produto adicionado!")

elif aba == "🧾 Pedidos":
    st.header("Pedidos")
    pedidos = conn.execute("""
        SELECT p.id, c.nome AS cliente, p.data, p.total
        FROM pedidos p
        JOIN clientes c ON p.cliente_id = c.id
    """).df()
    st.dataframe(pedidos)

    with st.form("novo_pedido"):
        st.subheader("Novo Pedido")

        clientes = conn.execute("SELECT id, nome FROM clientes").fetchall()
        produtos = conn.execute("SELECT id, nome, preco FROM produtos").fetchall()

        cliente = st.selectbox("Cliente", clientes, format_func=lambda x: x[1])
        data = datetime.now().strftime("%Y-%m-%d")
        itens = []

        st.markdown("### Itens do Pedido")
        for i in range(3):  # permitir até 3 itens
            cols = st.columns([3, 2])
            with cols[0]:
                prod = st.selectbox(f"Produto {i+1}", produtos, index=0, key=f"prod{i}", format_func=lambda x: x[1])
            with cols[1]:
                qtd = st.number_input(f"Qtd {i+1}", min_value=0, step=1, key=f"qtd{i}")
            if qtd > 0:
                itens.append((prod[0], prod[2], qtd))  # id, preco_unit, qtd

        submitted = st.form_submit_button("Salvar Pedido")
        if submitted and itens:
            total = sum(p[1] * p[2] for p in itens)
            conn.execute("INSERT INTO pedidos VALUES ((SELECT COALESCE(MAX(id), 0) + 1 FROM pedidos), ?, ?, ?)",
                         (cliente[0], data, total))
            pedido_id = conn.execute("SELECT MAX(id) FROM pedidos").fetchone()[0]
            for prod_id, preco_unit, qtd in itens:
                conn.execute("INSERT INTO itens_pedido VALUES (?, ?, ?, ?)",
                             (pedido_id, prod_id, qtd, preco_unit))
                conn.execute("UPDATE produtos SET estoque = estoque - ? WHERE id = ?", (qtd, prod_id))
            st.success(f"Pedido {pedido_id} registrado com sucesso!")

# Encerrar conexão
conn.close()
