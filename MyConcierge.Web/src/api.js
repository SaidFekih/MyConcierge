import { Notify } from 'quasar'
import { session, fermerSession } from './session'

// Petit helper pour appeler l'API .NET (le proxy de Vite redirige /api vers le backend)
async function appeler(methode, url, corps) {
  // Le jeton de connexion est envoyé à chaque appel
  const entetes = {}
  if (corps) entetes['Content-Type'] = 'application/json'
  if (session.jeton) entetes['Authorization'] = 'Bearer ' + session.jeton

  const reponse = await fetch('/api' + url, {
    method: methode,
    headers: entetes,
    body: corps ? JSON.stringify(corps) : undefined
  })

  // Jeton expiré ou refusé : on retourne à la page de connexion
  // (sans jeton, c'est un mauvais mot de passe : le message du serveur est affiché plus bas)
  if (reponse.status === 401 && session.jeton) {
    fermerSession()
    throw new Error('Session expirée, reconnecte-toi.')
  }

  if (reponse.status === 429) {
    throw new Error('Trop d\'essais, réessaie dans une minute.')
  }

  if (reponse.status >= 500) {
    throw new Error('Erreur du serveur (' + reponse.status + ')')
  }

  if (!reponse.ok) {
    // Le backend renvoie soit un texte simple (nos règles), soit un JSON de validation
    let message = await reponse.text()
    try { message = JSON.parse(message).title || message } catch { /* texte simple */ }
    throw new Error(message || 'Erreur ' + reponse.status)
  }

  return reponse.status === 204 ? null : reponse.json()
}

export const api = {
  get: (url) => appeler('GET', url),
  post: (url, corps) => appeler('POST', url, corps),
  put: (url, corps) => appeler('PUT', url, corps),
  delete: (url) => appeler('DELETE', url)
}

export function afficherErreur(erreur) {
  Notify.create({ type: 'negative', message: erreur.message })
}

export function afficherSucces(message) {
  Notify.create({ type: 'positive', message })
}
