<script setup>
import { ref, nextTick, onMounted } from 'vue'
import { api, afficherErreur } from '../api'

const types = ref([])
const categoriesPossibles = ['Unite', 'Utilisateur']
const formulaire = ref(creerFormulaireVide())
const formulaireRef = ref(null)
const idTypeEnModification = ref(null) // null = on ajoute un nouveau type

const colonnes = [
  { name: 'nom', label: 'Nom', field: 'nom', align: 'left' },
  { name: 'categorie', label: 'Catégorie', field: 'categorie', align: 'left' },
  { name: 'actions', label: '', align: 'right' }
]

function creerFormulaireVide() {
  return { nom: '', categorie: null }
}

async function chargerDonnees() {
  try {
    types.value = await api.get('/referencetypes')
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

async function viderFormulaire() {
  formulaire.value = creerFormulaireVide()
  idTypeEnModification.value = null
  await nextTick()
  formulaireRef.value.resetValidation()
}

// Remplit le formulaire avec le type choisi
function commencerModification(type) {
  idTypeEnModification.value = type.id
  formulaire.value = { nom: type.nom, categorie: type.categorie }
}

async function enregistrerType() {
  try {
    if (idTypeEnModification.value) {
      await api.put('/referencetypes/' + idTypeEnModification.value, {
        id: idTypeEnModification.value,
        ...formulaire.value
      })
    } else {
      await api.post('/referencetypes', formulaire.value)
    }
    await viderFormulaire()
    await chargerDonnees()
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

async function supprimerType(type) {
  if (!confirm('Supprimer le type « ' + type.nom + ' » ?')) return
  try {
    await api.delete('/referencetypes/' + type.id)
    await chargerDonnees()
  } catch (erreur) {
    afficherErreur(erreur) // ex : « Ce type est utilisé par des unités ou des utilisateurs… »
  }
}

onMounted(chargerDonnees)
</script>

<template>
  <div>
    <q-form ref="formulaireRef" @submit="enregistrerType" class="row q-col-gutter-md q-mb-md">
      <q-input v-model="formulaire.nom" label="Nom" dense outlined class="col-12 col-md-4"
               :rules="[(valeur) => !!valeur || 'Requis']" />
      <q-select v-model="formulaire.categorie" :options="categoriesPossibles" label="Catégorie" dense outlined
                class="col-12 col-md-4" :rules="[(valeur) => !!valeur || 'Requis']" />
      <div class="col-12 col-md-4">
        <q-btn type="submit" color="primary" :label="idTypeEnModification ? 'Enregistrer' : 'Ajouter'" />
        <q-btn v-if="idTypeEnModification" flat label="Annuler" @click="viderFormulaire" />
      </div>
    </q-form>

    <q-table :rows="types" :columns="colonnes" row-key="id" flat bordered title="Types">
      <template v-slot:body-cell-actions="props">
        <q-td :props="props">
          <q-btn flat color="primary" label="Modifier" @click="commencerModification(props.row)" />
          <q-btn flat color="negative" label="Supprimer" @click="supprimerType(props.row)" />
        </q-td>
      </template>
    </q-table>
  </div>
</template>