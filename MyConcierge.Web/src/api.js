import { Notify } from 'quasar'

// Petit helper pour appeler l'API .NET (le proxy de Vite redirige /api vers le backend)
async function appeler(methode, url, corps) {
  const reponse = await fetch('/api' + url, {
    method: methode,
    headers: corps ? { 'Content-Type': 'application/json' } : undefined,
    body: corps ? JSON.stringify(corps) : undefined
  })

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
  delete: (url) => appeler('DELETE', url)
}

export function afficherErreur(erreur) {
  Notify.create({ type: 'negative', message: erreur.message })
}
