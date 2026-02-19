-- Creazione del database (opzionale se lo hai già)
CREATE DATABASE IF NOT EXISTS cinebit_db;
USE cinebit_db;

-- 1. Tabella UTENTI
CREATE TABLE utenti (
    id_utente INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(50) NOT NULL,
    cognome VARCHAR(50) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL, -- Per contenere l'hash BCrypt o Argon2
    stato TINYINT(1) DEFAULT 1,     -- 1 = Attivo, 0 = Non attivo
    ruolo VARCHAR(20) DEFAULT 'user', -- 'admin' o 'user'
    data_ultima_modifica TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    utente_ultima_modifica INT,
    CONSTRAINT fk_utente_modifica FOREIGN KEY (utente_ultima_modifica) REFERENCES utenti(id_utente)
);

-- 2. Tabella PREFERITI
-- Nota: tmdb_id è l'ID che arriva dalle API esterne.
CREATE TABLE preferiti (
    id_prefe INT AUTO_INCREMENT PRIMARY KEY,
    id_utente INT NOT NULL,
    tmdb_id INT NOT NULL, 
    titolo_cache VARCHAR(255), -- Salviamo il titolo per evitare chiamate API superflue in lista
    data_aggiunta TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_preferiti_utente FOREIGN KEY (id_utente) REFERENCES utenti(id_utente) ON DELETE CASCADE,
    -- Indice unico composito: un utente non può salvare lo stesso film due volte
    UNIQUE KEY unique_user_movie (id_utente, tmdb_id)
);

-- 3. Tabella CHAT
CREATE TABLE chat (
    id_chat INT AUTO_INCREMENT PRIMARY KEY,
    id_utente INT NOT NULL,
    prompt TEXT NOT NULL,
    response TEXT NOT NULL,
    data_creazione TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_chat_utente FOREIGN KEY (id_utente) REFERENCES utenti(id_utente) ON DELETE CASCADE
);