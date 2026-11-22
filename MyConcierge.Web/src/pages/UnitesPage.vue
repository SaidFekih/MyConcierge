<script setup>
import { ref, nextTick, onMounted } from 'vue'
import { api, afficherErreur } from '../api'
import UniteDetails from '../components/UniteDetails.vue'

const unites = ref([])
const typesUnite = ref([])
const proprietaires = ref([])
const locataires = ref([])
const formulaire = ref(creerFormulaireVide())
const formulaireRef = ref(null)
const uniteSelectionnee = ref(null)
const fenetreOuverte = ref(false)

const colonnes = [
  { name: 'nom', label: 'Nom', field: 'nom', align: 'left' },
  { name: 'type', label: 'Type', field: (unite) => unite.referenceType?.nom, align: 'left' },
  { name: 'proprietaire', label: 'Propriétaire', field: (unite) => unite.proprietaire?.nom, align: 'left' },
  { name: 'prix', label: 'Prix', field: 'prix', align: 'right' },
  { name: 'estLouee', label: 'Louée', field: (unite) => (unite.estLouee ? 'Oui' : 'Non'), align: 'center' }
]

function creerFormulaireVide() {
  return { nom: '', referenceTypeId: null, proprietaireId: null, prix: 0, estLouee: false }
}

async function chargerDonnees() {
  try {
    unites.value = await api.get('/unites')
    const tousLesTypes = await api.get('/referencetypes')
    typesUnite.value = tousLesTypes.filter((type) => type.categorie === 'Unite')
    const utilisateurs = await api.get('/utilisateurs')
    // Seuls les propriétaires peuvent posséder une unité, seuls les locataires peuvent louer
    proprietaires.value = utilisateurs.filter((utilisateur) => utilisateur.referenceType?.nom === 'Propriétaire')
    locataires.value = utilisateurs.filter((utilisateur) => utilisateur.referenceType?.nom === 'Locataire')
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

async function ajouterUnite() {
  try {
    await api.post('/unites', formulaire.value)
    formulaire.value = creerFormulaireVide()
    await nextTick()
    formulaireRef.value.resetValidation() // évite les champs rouges après le vidage
    await chargerDonnees()
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

// Un clic sur une ligne ouvre la fenêtre de l'unité
function ouvrirFenetre(evenement, unite) {
  uniteSelectionnee.value = unite
  fenetreOuverte.value = true
}

// Après une action dans la fenêtre : recharge la liste et remet à jour l'unité affichée
async function rafraichirFenetre() {
  await chargerDonnees()
  uniteSelectionnee.value = unites.value.find((unite) => unite.id === uniteSelectionnee.value.id)
}

// Après la suppression de l'unité : ferme la fenêtre et recharge la liste
async function fermerFenetreApresSuppression() {
  fenetreOuverte.value = false
  await chargerDonnees()
}

onMounted(chargerDonnees)
</script>

<template>
  <div>
    <q-form ref="formulaireRef" @submit="ajouterUnite" class="row q-col-gutter-md q-mb-md">
      <q-input v-model="formulaire.nom" label="Nom" dense outlined class="col-12 col-md-3"
               :rules="[(valeur) => !!valeur || 'Requis']" />
      <q-select v-model="formulaire.referenceTypeId" :options="typesUnite" option-value="id" option-label="nom"
                emit-value map-options label="Type" dense outlined class="col-12 col-md-2"
                :rules="[(valeur) => !!valeur || 'Requis']" />
      <q-select v-model="formulaire.proprietaireId" :options="proprietaires" option-value="id" option-label="nom"
                emit-value map-options label="Propriétaire" dense outlined class="col-12 col-md-3"
                :rules="[(valeur) => !!valeur || 'Requis']" />
      <q-input v-model.number="formulaire.prix" type="number" label="Prix" dense outlined class="col-12 col-md-2" />
      <div class="col-12 col-md-2">
        <q-btn type="submit" color="primary" label="Ajouter" />
      </div>
    </q-form>

    <q-table :rows="unites" :columns="colonnes" row-key="id" flat bordered title="Unités"
             @row-click="ouvrirFenetre" />

    <q-dialog v-model="fenetreOuverte">
      <UniteDetails v-if="uniteSelectionnee" :unite="uniteSelectionnee" :types-unite="typesUnite"
                    :proprietaires="proprietaires" :locataires="locataires"
                    @donnees-modifiees="rafraichirFenetre" @unite-supprimee="fermerFenetreApresSuppression" />
    </q-dialog>
  </div>
</template>
