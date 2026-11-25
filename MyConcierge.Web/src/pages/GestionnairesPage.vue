<script setup>
import { ref, nextTick, onMounted } from 'vue'
import { api, afficherErreur, afficherSucces } from '../api'
import { session } from '../session'
import { formaterDate } from '../format'

const comptes = ref([])
const formulaire = ref(creerFormulaireVide())
const formulaireRef = ref(null)

const colonnes = [
  { name: 'nom', label: 'Nom', field: 'nom', align: 'left' },
  { name: 'courriel', label: 'Courriel', field: 'courriel', align: 'left' },
  { name: 'dateCreation', label: 'Créé le', field: (compte) => formaterDate(compte.dateCreation), align: 'left' },
  { name: 'statut', label: 'Statut', align: 'left' },
  { name: 'actions', label: '', align: 'right' }
]

function creerFormulaireVide() {
  return { nom: '', courriel: '', motDePasse: '' }
}

function estMonCompte(compte) {
  return compte.courriel === session.courriel
}

async function chargerComptes() {
  try {
    comptes.value = await api.get('/comptes')
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

async function creerCompte() {
  try {
    await api.post('/comptes', formulaire.value)
    afficherSucces('Compte créé pour ' + formulaire.value.nom)
    formulaire.value = creerFormulaireVide()
    await nextTick()
    formulaireRef.value.resetValidation()
    await chargerComptes()
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

async function changerStatut(compte) {
  const action = compte.estActif ? 'desactiver' : 'reactiver'
  try {
    await api.post('/comptes/' + compte.id + '/' + action)
    await chargerComptes()
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

onMounted(chargerComptes)
</script>

<template>
  <div>
    <q-form ref="formulaireRef" @submit="creerCompte" class="row q-col-gutter-md q-mb-md">
      <q-input v-model="formulaire.nom" label="Nom" dense outlined class="col-12 col-md-3"
               :rules="[(valeur) => !!valeur || 'Requis']" />
      <q-input v-model="formulaire.courriel" type="email" label="Courriel" dense outlined class="col-12 col-md-3"
               :rules="[(valeur) => !!valeur || 'Requis']" />
      <q-input v-model="formulaire.motDePasse" type="password" label="Mot de passe (10 caractères minimum)" dense outlined
               autocomplete="new-password" class="col-12 col-md-4"
               :rules="[(valeur) => (valeur && valeur.length >= 10) || '10 caractères minimum']" />
      <div class="col-12 col-md-2">
        <q-btn type="submit" color="primary" label="Créer le compte" />
      </div>
    </q-form>

    <q-table :rows="comptes" :columns="colonnes" row-key="id" flat bordered title="Gestionnaires"
             hide-pagination :rows-per-page-options="[0]">
      <template v-slot:body-cell-statut="props">
        <q-td :props="props">
          <q-badge :color="props.row.estActif ? 'positive' : 'grey'" :label="props.row.estActif ? 'Actif' : 'Désactivé'" />
          <q-badge v-if="estMonCompte(props.row)" outline color="primary" label="Toi" class="q-ml-sm" />
        </q-td>
      </template>
      <template v-slot:body-cell-actions="props">
        <q-td :props="props">
          <q-btn v-if="props.row.estActif" flat color="negative" label="Désactiver"
                 :disable="estMonCompte(props.row)" @click="changerStatut(props.row)" />
          <q-btn v-else flat color="primary" label="Réactiver" @click="changerStatut(props.row)" />
        </q-td>
      </template>
    </q-table>
  </div>
</template>
