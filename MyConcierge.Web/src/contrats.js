// Règles de dates des contrats, partagées par les pages

export function dateDuJour() {
  return new Date().toLocaleDateString('en-CA') // AAAA-MM-JJ, heure locale
}

// Le jour suivant une date AAAA-MM-JJ (ex : "2026-12-31" → "2027-01-01")
export function lendemainDe(dateTexte) {
  const [annee, mois, jour] = dateTexte.slice(0, 10).split('-').map(Number)
  return new Date(annee, mois - 1, jour + 1).toLocaleDateString('en-CA')
}

export function estAVenir(contrat) {
  return contrat.dateDebut.slice(0, 10) > dateDuJour()
}

// En cours : commencé, et pas encore terminé
export function estEnCours(contrat) {
  return !estAVenir(contrat) && (!contrat.dateFin || contrat.dateFin.slice(0, 10) >= dateDuJour())
}
