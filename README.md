# CineBit
# Database – Versione 1

## Schema del Database
Il database di CineBit è composto da quattro tabelle principali:
film, utenti, preferiti, chat.

## Tabella: film
Contiene i dati dei film importati da TMDB.

Campi
- Campo	Tipo	Descrizione
- adult	BOOL	Indica se il film è per adulti
- backdrop_path	TEXT	Percorso immagine di sfondo
- genre_ids	ARRAY(INT)	Lista ID dei generi
- id	INT	ID univoco del film (TMDB)
- original_language	TEXT	Lingua originale
- original_title	TEXT	Titolo originale
- overview	TEXT	Trama
- popularity	FLOAT	Popolarità
- poster_path	TEXT	Percorso poster
- release_date	DATE	Data di uscita
- title	TEXT	Titolo
- video	BOOL	Indica se è un video
- vote_average	FLOAT	Media voti
- vote_count	INT	Numero voti
## Tabella: utenti
- Campi
- Campo	Tipo	Descrizione
- id_utente	INT	ID univoco utente
- nome	TEXT	Nome
- cognome	TEXT	Cognome
- mail	TEXT	Email
- password	TEXT (hash)	Password criptata
- stato	BOOL	Attivo = true / Non attivo = false
- ruolo	BOOL	Admin = true / Utente = false
- data_ultima_modifica	TIMESTAMP	Data ultima modifica
- utente_ultima_modifica	INT	ID utente che ha effettuato la modifica
## Tabella: preferiti
- Campi
- Campo	Tipo	Descrizione
- id_prefe	INT	ID univoco preferito
- titolo	TEXT	Titolo del film salvato
- id_utente	INT (FK)	Utente proprietario
- id_film	INT (FK)	Film salvato
## Tabella: chat
- Campi
- Campo	Tipo	Descrizione
- id_chat	INT	ID univoco chat
- prompt	TEXT	Input dell’utente
- response	TEXT	Risposta generata
