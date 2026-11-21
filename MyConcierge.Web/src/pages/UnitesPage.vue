<script setup>
import { ref, nextTick, onMounted } from 'vue'
import { api, afficherErreur } from '../api'

const unites = ref([])
const typesUnite = ref([])
const proprietaires = ref([])
const formulaire = ref(creerFormulaireVide())
const formulaireRef = ref(null)
const idUniteEnModification = ref(null) // null = on ajoute une nouvelle unité

const colonnes = [
  { name: 'nom', label: 'Nom', field: 'nom', align: 'left' },
  { name: 'type', label: 'Type', field: (unite) => unite.referenceType?.nom, align: 'left' },
  { name: 'proprietaire', label: 'Propriétaire', field: (unite) => unite.proprietaire?.nom, align: 'left' },
  { name: 'prix', label: 'Prix', field: 'prix', align: 'right' },
  { name: 'estLouee', label: 'Louée', field: (unite) => (unite.estLouee ? 'Oui' : 'Non'), align: 'center' },
  { name: 'actions', label: '', align: 'right' }
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
    // Seuls les propriétaires peuvent posséder une unité
    proprietaires.value = utilisateurs.filter((utilisateur) => utilisateur.referenceType?.nom === 'Propriétaire')
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

async function viderFormulaire() {
  formulaire.value = creerFormulaireVide()
  idUniteEnModification.value = null
  await nextTick()
  formulaireRef.value.resetValidation() // évite les champs rouges après le vidage
}

// Remplit le formulaire avec l'unité choisie
function commencerModification(unite) {
  idUniteEnModification.value = unite.id
  formulaire.value = {
    nom: unite.nom,
    referenceTypeId: unite.referenceTypeId,
    proprietaireId: unite.proprietaireId,
    prix: unite.prix,
    estLouee: unite.estLouee // conservé tel quel : ce sont les contrats qui le changent
  }
}

async function enregistrerUnite() {
  try {
    if (idUniteEnModification.value) {
      await api.put('/unites/' + idUniteEnModification.value, {
        id: idUniteEnModification.value,
        ...formulaire.value
      })
    } else {
      await api.post('/unites', formulaire.value)
    }
    await viderFormulaire()
    await chargerDonnees()
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

async function supprimerUnite(unite) {
  if (!confirm('Supprimer l\'unité « ' + unite.nom + ' » ?')) return
  try {
    await api.delete('/unites/' + unite.id)
    await chargerDonnees()
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

onMounted(chargerDonnees)
</script>

<template>
  <div>
    <q-form ref="formulaireRef" @submit="enregistrerUnite" class="row q-col-gutter-md q-mb-md">
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
        <q-btn type="submit" color="primary" :label="idUniteEnModification ? 'Enregistrer' : 'Ajouter'" />
        <q-btn v-if="idUniteEnModification" flat label="Annuler" @click="viderFormulaire" />
      </div>
    </q-form>

    <q-table :rows="unites" :columns="colonnes" row-key="id" flat bordered title="Unités">
      <template v-slot:body-cell-actions="props">
        <q-td :props="props">
          <q-btn flat color="primary" label="Modifier" @click="commencerModification(props.row)" />
          <q-btn flat color="negative" label="Supprimer" @click="supprimerUnite(props.row)" />
        </q-td>
      </template>
    </q-table>
  </div>
</template>