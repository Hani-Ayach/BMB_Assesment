# 🧾 .NET Microservices Order Platform

A full-stack order-taking platform built with .NET microservices and Razor Pages, featuring:

- Product and Order APIs
- API Gateway with routing, JWT Auth, Rate Limiting, Health Checks
- Razor Frontend (UI)
- Dockerized with  In-Memory support
- Serilog Structured Logging

---

## 🏗️ Architecture

```text
                +-------------+
                |   Gateway   | <--- JWT, Rate Limiting, Routing
                +------+------+
                       |
    +------------------+------------------+
    |                                     |
+---v---+                           +-----v----+
|Product|                           |  Order   |
|Service|                           | Service  |
+---+---+                           +-----+----+
    |                                     |
    |              +----------------------+  
    |              |
+---v--------------v---+
|   Shared Class Library |
+------------------------+



The Microservice Application is Not Totaly finish
The Application need more testing, and some patterns implementations
