// Fonctions d'affichage partagées par les pages

// 1250 -> "1 250,00 $"
export function formaterMontant(montant) {
  return new Intl.NumberFormat('fr-CA', { style: 'currency', currency: 'CAD' }).format(montant)
}

// "2026-11-21T00:00:00" ou "2026-11-21" -> "21 nov. 2026"
export function formaterDate(dateTexte) {
  // On force l'heure locale pour éviter un décalage d'un jour
  const date = new Date(dateTexte.slice(0, 10) + 'T00:00:00')
  return date.toLocaleDateString('fr-CA', { day: 'numeric', month: 'short', year: 'numeric' })
}
