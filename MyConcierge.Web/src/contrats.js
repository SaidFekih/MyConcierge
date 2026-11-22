// Règles de dates des contrats, partagées par les pages

export function dateDuJour() {
  return new Date().toLocaleDateString('en-CA') // AAAA-MM-JJ, heure locale
}

export function estAVenir(contrat) {
  return contrat.dateDebut.slice(0, 10) > dateDuJour()
}

// En cours : commencé, et pas encore terminé
export function estEnCours(contrat) {
  return !estAVenir(contrat) && (!contrat.dateFin || contrat.dateFin.slice(0, 10) >= dateDuJour())
}
