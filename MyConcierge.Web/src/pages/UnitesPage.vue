<script setup>
import { ref, nextTick, onMounted } from 'vue'
import { api, afficherErreur } from '../api'

const unites = ref([])
const types = ref([])
const utilisateurs = ref([])
const formulaire = ref(formulaireVide())
const formulaireRef = ref(null)

const colonnes = [
  { name: 'nom', label: 'Nom', field: 'nom', align: 'left' },
  { name: 'type', label: 'Type', field: (u) => u.referenceType?.nom, align: 'left' },
  { name: 'proprietaire', label: 'Propriétaire', field: (u) => u.proprietaire?.nom, align: 'left' },
  { name: 'prix', label: 'Prix', field: 'prix', align: 'right' },
  { name: 'estLouee', label: 'Louée', field: (u) => (u.estLouee ? 'Oui' : 'Non'), align: 'center' },
  { name: 'actions', label: '', align: 'right' }
]

function formulaireVide() {
  return { nom: '', referenceTypeId: null, proprietaireId: null, prix: 0 }
}

async function charger() {
  try {
    unites.value = await api.get('/unites')
    const tousLesTypes = await api.get('/referencetypes')
    types.value = tousLesTypes.filter((t) => t.categorie === 'Unite')
    const tousLesUtilisateurs = await api.get('/utilisateurs')
    // Seuls les propriétaires peuvent posséder une unité
    utilisateurs.value = tousLesUtilisateurs.filter((u) => u.referenceType?.nom === 'Propriétaire')
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

async function ajouter() {
  try {
    await api.post('/unites', { ...formulaire.value, estLouee: false })
    formulaire.value = formulaireVide()
    await nextTick()
    formulaireRef.value.resetValidation() // évite les champs rouges après le vidage
    await charger()
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

async function supprimer(unite) {
  if (!confirm('Supprimer l\'unité « ' + unite.nom + ' » ?')) return
  try {
    await api.delete('/unites/' + unite.id)
    await charger()
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

onMounted(charger)
</script>

<template>
  <div>
    <q-form ref="formulaireRef" @submit="ajouter" class="row q-col-gutter-md q-mb-md">
      <q-input v-model="formulaire.nom" label="Nom" dense outlined class="col-12 col-md-3"
               :rules="[(v) => !!v || 'Requis']" />
      <q-select v-model="formulaire.referenceTypeId" :options="types" option-value="id" option-label="nom"
                emit-value map-options label="Type" dense outlined class="col-12 col-md-3"
                :rules="[(v) => !!v || 'Requis']" />
      <q-select v-model="formulaire.proprietaireId" :options="utilisateurs" option-value="id" option-label="nom"
                emit-value map-options label="Propriétaire" dense outlined class="col-12 col-md-3"
                :rules="[(v) => !!v || 'Requis']" />
      <q-input v-model.number="formulaire.prix" type="number" label="Prix" dense outlined class="col-12 col-md-2" />
      <div class="col-12 col-md-1">
        <q-btn type="submit" color="primary" label="Ajouter" />
      </div>
    </q-form>

    <q-table :rows="unites" :columns="colonnes" row-key="id" flat bordered title="Unités">
      <template v-slot:body-cell-actions="props">
        <q-td :props="props">
          <q-btn flat color="negative" label="Supprimer" @click="supprimer(props.row)" />
        </q-td>
      </template>
    </q-table>
  </div>
</template>
