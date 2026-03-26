# 🔐 Authentication Learning Project

โปรเจกต์นี้สร้างขึ้นเพื่อ **สอนการ Authentication รูปแบบต่าง ๆ** ใน ASP.NET Core Web API  
แต่ละ sub-project จะสาธิตวิธีการ Auth ที่แตกต่างกัน เหมาะสำหรับการเปรียบเทียบและทำความเข้าใจแนวคิดของแต่ละรูปแบบ

---

## 📁 โครงสร้าง Project

```
authen/
├── auth.models/          # Shared Library: Entity, DbContext, DTO ใช้ร่วมกันทุก project
│   ├── DB/               # Entity (TBL_User, TBL_Hmac)
│   ├── Data/             # DataContext (EF Core)
│   ├── Dto/              # Data Transfer Objects
│   └── Migrations/       # EF Core Migrations
│
├── auth.basic/           # Project 1: Basic Authentication
├── auth.hmac/            # Project 2: HMAC Authentication
└── auth.jwt/             # Project 3: JWT Authentication
```

---

## 🟡 Project 1 — Basic Authentication (`auth.basic`)

### แนวคิด
ส่ง `username:password` ใน HTTP Header โดย encode ด้วย **Base64**

```
Authorization: Basic dXNlcm5hbWU6cGFzc3dvcmQ=
```

### การทำงาน
1. Client ส่ง `Authorization` header มากับ Request
2. `BasicAuthMiddleware` ดัก header และ decode Base64
3. ตรวจสอบ username/password กับฐานข้อมูล
4. ถ้าถูกต้อง → inject `ClaimsIdentity` เข้า `HttpContext.User`
5. Controller ที่ใช้ `[Authorize]` จะได้รับสิทธิ์เข้าถึง

### API Endpoints

| Method | Path | Auth Required | คำอธิบาย |
|--------|------|:---:|-----------|
| GET | `/api/User/GetOk` | ❌ | Health check |
| GET | `/api/User/GetAllUser` | ✅ | ดึงรายชื่อ user ทั้งหมด |

### ไฟล์สำคัญ
- `Middleware/BasicAuthMiddleware.cs` — ตรวจสอบ Basic Auth header
- `Services/UserService.cs` — logic ยืนยัน username/password
- `Program.cs` — ลงทะเบียน Middleware และ DbContext

> ⚠️ **ข้อควรระวัง:** Basic Auth ส่ง credential เป็น Base64 (ไม่ใช่ encryption) ควรใช้ **HTTPS เท่านั้น**

---

## 🟠 Project 2 — HMAC Authentication (`auth.hmac`)

### แนวคิด
ใช้ **HMAC-SHA256** สร้าง signature จาก `timestamp + service_name + secret_key`  
เหมาะสำหรับ **API-to-API communication** (Machine-to-Machine)

```
Headers:
  hmac: <hmac_signature>
  timestamp: <unix_timestamp>
  sercice_name: HRM
```

### การทำงาน
1. Client เรียก `GET /api/User/GetHmac` เพื่อรับ HMAC + Timestamp
2. Client นำ HMAC ไปแนบใน Header ของ Request ถัดไป
3. Server สร้าง HMAC เองจาก `timestamp + service_name + secret_key`
4. เปรียบเทียบ HMAC ที่ได้รับกับที่คำนวณ
5. ตรวจสอบ timestamp ว่ายังไม่หมดอายุ (Replay Attack prevention)

### API Endpoints

| Method | Path | Auth Required | คำอธิบาย |
|--------|------|:---:|-----------|
| GET | `/api/User/GetHmac` | ❌ | รับ HMAC token สำหรับใช้งาน |
| GET | `/api/User/GetName` | ✅ | ทดสอบ HMAC Auth (ต้องส่ง header) |
| GET | `/api/User/GetNameAuth` | ✅ | Protected endpoint |

### ไฟล์สำคัญ
- `Extensions/HmacExtensions.cs` — สูตรคำนวณ HMAC-SHA256
- `Middleware/AuthenticationMiddleware.cs` — ตรวจสอบ HMAC header
- `Services/AuthenticationService.cs` — ตรวจสอบ token กับฐานข้อมูล

> 💡 **Secret Key** ใน Production ควรเก็บไว้ใน `appsettings.json` หรือ Environment Variable ไม่ควร hardcode ใน code

---

## 🟢 Project 3 — JWT Authentication (`auth.jwt`)

### แนวคิด
ใช้ **JSON Web Token (JWT)** เป็น stateless authentication  
Client เก็บ token ไว้เอง Server ไม่ต้องเก็บ session

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### การทำงาน
1. **Register** — สร้าง user ใหม่ โดย hash password ด้วย `HMACSHA512`
2. **Login** — ตรวจสอบ password → คืน `Access Token` + `Refresh Token`
3. **Access Token** (อายุสั้น) ใช้ยืนยันตัวตนกับ API
4. **Refresh Token** (อายุ 1 วัน) ใช้ขอ Access Token ใหม่เมื่อหมดอายุ
5. ทุก `[Authorize]` endpoint — Middleware ตรวจสอบ JWT signature อัตโนมัติ

### API Endpoints

| Method | Path | Auth Required | คำอธิบาย |
|--------|------|:---:|-----------|
| POST | `/api/Auth/Register` | ❌ | สมัครสมาชิกใหม่ |
| POST | `/api/Auth/login` | ❌ | เข้าสู่ระบบ รับ JWT |
| POST | `/api/Auth/Refresh` | ❌ | ขอ Access Token ใหม่ |
| GET | `/api/User/GetAllUser` | ✅ | ดึงรายชื่อ user ทั้งหมด |

### ไฟล์สำคัญ
- `Controllers/AuthController.cs` — Login, Register, Refresh Token
- `Services/TokenService.cs` — สร้าง/ตรวจสอบ JWT
- `Extensions/` — ลงทะเบียน Identity services

---

## 🗄️ Shared Library — `auth.models`

ใช้ร่วมกันทั้ง 3 project ประกอบด้วย:

| โฟลเดอร์ | คำอธิบาย |
|----------|-----------|
| `DB/TBL_User.cs` | Entity สำหรับตาราง User (Username, PasswordHash, PasswordSalt, RefreshToken) |
| `DB/TBL_Hmac.cs` | Entity สำหรับตาราง HMAC token |
| `Data/DataContext.cs` | EF Core DbContext |
| `Dto/` | DTO สำหรับ Request/Response (LoginDto, JwtDto, UserDto, HmacDto) |

---

## ⚙️ การตั้งค่าก่อนรัน

### 1. ติดตั้ง SQL Server
จำเป็นสำหรับ `auth.basic` และ `auth.jwt`

### 2. แก้ Connection String
ใน `Program.cs` ของ `auth.basic`:
```csharp
string Constr = "Data Source=localhost;Initial Catalog=authDB;User Id=sa;Password=Abc@12345;...";
```

ปรับ `User Id` และ `Password` ให้ตรงกับ SQL Server ของตัวเอง

### 3. สร้างฐานข้อมูลด้วย EF Core Migration
```bash
# รันใน Project ที่ต้องการ
dotnet ef database update
```

### 4. เปิด Swagger UI
เมื่อรัน Project แล้ว เข้า:
```
https://localhost:{port}/swagger
```

---

## 📊 เปรียบเทียบ 3 รูปแบบ

| คุณสมบัติ | Basic Auth | HMAC Auth | JWT Auth |
|-----------|:----------:|:---------:|:--------:|
| ใช้งานง่าย | ✅ มาก | ⚠️ ปานกลาง | ⚠️ ปานกลาง |
| ความปลอดภัย | ⚠️ ต่ำ | ✅ สูง | ✅ สูง |
| Stateless | ✅ | ✅ | ✅ |
| Expiry support | ❌ | ✅ | ✅ |
| เหมาะกับ | Web แบบง่าย | API-to-API | Web/Mobile App |

---

## 🛠️ Tech Stack

- **Framework:** ASP.NET Core Web API (.NET 8)
- **ORM:** Entity Framework Core + Lazy Loading
- **Database:** SQL Server
- **Security:** HMACSHA512, HMACSHA256, JWT Bearer
- **Docs:** Swagger / OpenAPI
