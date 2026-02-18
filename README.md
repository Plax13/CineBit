# CineBit



# db versione 1
/*
schema db cinebit 

- tabella film (preso da tmdb) -- > info su discord
	campi:
		"adult": false,
		"backdrop_path": "/zfbjgQE1uSd9wiPTX4VzsLi0rGG.jpg",
		"genre_ids": [
			18,
			80
		],
		"id": 278,
		"original_language": "en",
		"original_title": "The Shawshank Redemption",
		"overview": "Imprisoned in the 1940s for the double murder of his wife and her lover, upstanding banker Andy Dufresne begins a new life at the Shawshank prison, where he puts his accounting skills to work for an amoral warden. During his long stretch in prison, Dufresne comes to be admired by the other inmates -- including an older prisoner named Red -- for his integrity and unquenchable sense of hope.",
		"popularity": 39.8005,
		"poster_path": "/9cqNxx0GxF0bflZmeSMuL5tnGzr.jpg",
		"release_date": "1994-09-23",
		"title": "The Shawshank Redemption",
		"video": false,
		"vote_average": 8.715,
		"vote_count": 29744

- tabella utenti
	campi :
		id_utente
        nome
        cognome
        mail
        password (hash)
        stato -- > BOOL (attivo = true / non attivo = false)
        ruolo -- > BOOL(admin=true / utente=false)
        data_ultima_modifica
		utente_ultima_modifica
    
- tabella preferiti
	campi : 
		id_prefe
        titolo
        id_utente (FK)
        id_film(FK)
    
- tabella chat
	campi :
		id_chat
		prompt
        response
	
    

*/
