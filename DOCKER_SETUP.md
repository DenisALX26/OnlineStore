# Configurare Baza de Date cu Docker

## Opțiuni de configurare

### Opțiunea 1: Doar baza de date în Docker (Recomandat pentru dezvoltare locală)

1. **Pornește baza de date:**
   ```bash
   docker-compose up db -d
   ```

2. **Verifică că baza de date rulează:**
   ```bash
   docker ps
   ```

3. **Rulează aplicația local:**
   - Aplicația va folosi connection string-ul din `appsettings.json` care se conectează la `localhost:3306`

### Opțiunea 2: Totul în Docker (Aplicație + Baza de date)

1. **Construiește și pornește toate serviciile:**
   ```bash
   docker-compose up --build
   ```

2. **Sau în background:**
   ```bash
   docker-compose up --build -d
   ```

3. **Accesează aplicația:**
   - Aplicația va fi disponibilă la `http://localhost:8080`

## Configurare Baza de Date

### Credențiale MySQL:
- **Server:** `db` (în Docker) sau `localhost` (pentru conexiuni externe)
- **Port:** `3306`
- **Database:** `onlinestoredb`
- **User:** `storeuser`
- **Password:** `storeuserpass`
- **Root Password:** `rootpass`

### Connection String:
- **Pentru Docker:** `Server=db;Port=3306;Database=onlinestoredb;User=storeuser;Password=storeuserpass;`
- **Pentru localhost:** `Server=localhost;Port=3306;Database=onlinestoredb;User=storeuser;Password=storeuserpass;`

## Comenzi utile

### Verifică statusul containerelor:
```bash
docker-compose ps
```

### Vezi logurile:
```bash
docker-compose logs db
docker-compose logs web
```

### Oprește serviciile:
```bash
docker-compose down
```

### Oprește și șterge volume-ul (ATENȚIE: șterge datele):
```bash
docker-compose down -v
```

### Conectează-te la MySQL din Docker:
```bash
docker exec -it my_mysql_db_asp_dotnet mysql -u storeuser -pstoreuserpass onlinestoredb
```

## Migrații Entity Framework

### Când rulezi aplicația local (DB în Docker):
```bash
cd OnlineStoreApp
dotnet ef database update
```

### Când rulezi totul în Docker:
```bash
docker-compose exec web dotnet ef database update
```

## Depanare

### Dacă baza de date nu pornește:
1. Verifică dacă portul 3306 este liber: `netstat -an | findstr 3306`
2. Verifică logurile: `docker-compose logs db`
3. Șterge containerul și volume-ul și încearcă din nou: `docker-compose down -v`

### Dacă aplicația nu se poate conecta la baza de date:
1. Verifică că baza de date este healthy: `docker-compose ps`
2. Verifică connection string-ul în `appsettings.json` sau variabilele de mediu
3. Verifică că aplicația așteaptă ca baza de date să fie gata (depends_on în docker-compose.yml)

