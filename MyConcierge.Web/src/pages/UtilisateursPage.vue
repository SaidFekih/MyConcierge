<script setup>
import { ref, nextTick, onMounted } from 'vue'
import { api, afficherErreur } from '../api'

const utilisateurs = ref([])
const types = ref([])
const formulaire = ref(formulaireVide())
const formulaireRef = ref(null)

const colonnes = [
  { name: 'nom', label: 'Nom', field: 'nom', align: 'left' },
  { name: 'email', label: 'Email', field: 'email', align: 'left' },
  { name: 'telephone', label: 'Téléphone', field: 'telephone', align: 'left' },
  { name: 'type', label: 'Type', field: (u) => u.referenceType?.nom, align: 'left' },
  { name: 'actions', label: '', align: 'right' }
]

function formulaireVide() {
  return { nom: '', email: '', telephone: '', referenceTypeId: null }
}

async function charger() {
  try {
    utilisateurs.value = await api.get('/utilisateurs')
    const tousLesTypes = await api.get('/referencetypes')
    types.value = tousLesTypes.filter((t) => t.categorie === 'Utilisateur')
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

async function ajouter() {
  try {
    await api.post('/utilisateurs', { ...formulaire.value })
    formulaire.value = formulaireVide()
    await nextTick()
    formulaireRef.value.resetValidation()
    await charger()
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

async function supprimer(utilisateur) {
  if (!confirm('Supprimer « ' + utilisateur.nom + ' » ?')) return
  try {
    await api.delete('/utilisateurs/' + utilisateur.id)
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
      <q-input v-model="formulaire.email" type="email" label="Email" dense outlined class="col-12 col-md-3"
               :rules="[(v) => !!v || 'Requis']" />
      <q-input v-model="formulaire.telephone" label="Téléphone" dense outlined class="col-12 col-md-2" />
      <q-select v-model="formulaire.referenceTypeId" :options="types" option-value="id" option-label="nom"
                emit-value map-options label="Type" dense outlined class="col-12 col-md-3"
                :rules="[(v) => !!v || 'Requis']" />
      <div class="col-12 col-md-1">
        <q-btn type="submit" color="primary" label="Ajouter" />
      </div>
    </q-form>

    <q-table :rows="utilisateurs" :columns="colonnes" row-key="id" flat bordered title="Utilisateurs">
      <template v-slot:body-cell-actions="props">
        <q-td :props="props">
          <q-btn flat color="negative" label="Supprimer" @click="supprimer(props.row)" />
        </q-td>
      </template>
    </q-table>
  </div>
</template>