import { reactive } from 'vue'

// La session = la personne connectée et son jeton.
// On la garde dans le navigateur pour rester connecté après un rafraîchissement de la page.
const CLE_DE_STOCKAGE = 'myconcierge-session'

function lireSessionSauvegardee() {
  try {
    return JSON.parse(localStorage.getItem(CLE_DE_STOCKAGE)) || {}
  } catch {
    return {}
  }
}

const sauvegardee = lireSessionSauvegardee()

export const session = reactive({
  jeton: sauvegardee.jeton || null,
  nom: sauvegardee.nom || '',
  courriel: sauvegardee.courriel || ''
})

// Appelée après une connexion réussie (réponse de POST /api/auth/connexion)
export function ouvrirSession(reponse) {
  session.jeton = reponse.jeton
  session.nom = reponse.nom
  session.courriel = reponse.courriel
  try {
    localStorage.setItem(CLE_DE_STOCKAGE, JSON.stringify({ jeton: reponse.jeton, nom: reponse.nom, courriel: reponse.courriel }))
  } catch { /* le stockage peut être indisponible : on reste connecté jusqu'au rafraîchissement */ }
}

export function fermerSession() {
  session.jeton = null
  session.nom = ''
  session.courriel = ''
  try {
    localStorage.removeItem(CLE_DE_STOCKAGE)
  } catch { /* rien à faire */ }
}
