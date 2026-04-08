# 🧾 POS System API

API REST para un sistema de punto de venta con generación de facturas y análisis de ventas.

---

## 🚀 Tecnologías

- ⚙️ ASP.NET Core
- 🗄️ SQL Server (transaccional)
- 📊 PostgreSQL (analytics)
- 🧾 iText7 (generación de PDF)
- 🔐 Autenticación con jwt

---

## 📦 Funcionalidades

✔ Gestión de ventas  
✔ Generación de facturas (XML y PDF)  
✔ Descarga de archivos  
✔ Dashboard de análisis  
✔ Agrupación de ventas por día  
✔ Cálculo de impuestos y totales  

---

## 📊 Endpoints principales

### 🧾 Facturas
```http
GET /api/invoices
GET /api/invoices/{id}
GET /api/invoices/{id}/xml
GET /api/invoices/{id}/pdf
```

###📊 Analytics
```http
GET /api/analytics/daily
GET /api/analytics/summary
