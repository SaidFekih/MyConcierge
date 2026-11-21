<script setup>
import { ref, nextTick, onMounted } from 'vue'
import { api, afficherErreur } from '../api'

const contrats = ref([])
const unites = ref([])
const locataires = ref([])
const formulaire = ref(creerFormulaireVide())
const formulaireRef = ref(null)
const idContratEnModification = ref(null) // null = on ajoute un nouveau contrat

const colonnes = [
  { name: 'unite', label: 'Unité', field: (contrat) => contrat.unite?.nom, align: 'left' },
  { name: 'locataire', label: 'Locataire', field: (contrat) => contrat.locataire?.nom, align: 'left' },
  { name: 'dateDebut', label: 'Début', field: (contrat) => contrat.dateDebut.slice(0, 10), align: 'left' },
  { name: 'dateFin', label: 'Fin', field: (contrat) => (contrat.dateFin ? contrat.dateFin.slice(0, 10) : 'En cours'), align: 'left' },
  { name: 'montant', label: 'Montant', field: 'montant', align: 'right' },
  { name: 'actions', label: '', align: 'right' }
]

function creerFormulaireVide() {
  return { uniteId: null, locataireId: null, dateDebut: '', dateFin: '', montant: 0 }
}

async function chargerDonnees() {
  try {
    contrats.value = await api.get('/contrats')
    unites.value = await api.get('/unites')
    const utilisateurs = await api.get('/utilisateurs')
    // Seuls les locataires peuvent signer un contrat
    locataires.value = utilisateurs.filter((utilisateur) => utilisateur.referenceType?.nom === 'Locataire')
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

async function viderFormulaire() {
  formulaire.value = creerFormulaireVide()
  idContratEnModification.value = null
  await nextTick()
  formulaireRef.value.resetValidation() // évite les champs rouges après le vidage
}

// Remplit le formulaire avec le contrat choisi
function commencerModification(contrat) {
  idContratEnModification.value = contrat.id
  formulaire.value = {
    uniteId: contrat.uniteId,
    locataireId: contrat.locataireId,
    dateDebut: contrat.dateDebut.slice(0, 10),
    dateFin: contrat.dateFin ? contrat.dateFin.slice(0, 10) : '',
    montant: contrat.montant
  }
}

async function enregistrerContrat() {
  const donneesContrat = {
    ...formulaire.value,
    dateFin: formulaire.value.dateFin || null // vide = contrat sans fin
  }
  try {
    if (idContratEnModification.value) {
      await api.put('/contrats/' + idContratEnModification.value, { id: idContratEnModification.value, ...donneesContrat })
    } else {
      await api.post('/contrats', donneesContrat)
    }
    await viderFormulaire()
    await chargerDonnees()
  } catch (erreur) {
    afficherErreur(erreur) // ex : « Cette unité est déjà louée sur cette période. »
  }
}

async function supprimerContrat(contrat) {
  if (!confirm('Supprimer ce contrat ?')) return
  try {
    await api.delete('/contrats/' + contrat.id)
    await chargerDonnees()
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

onMounted(chargerDonnees)
</script>

<template>
  <div>
    <q-form ref="formulaireRef" @submit="enregistrerContrat" class="row q-col-gutter-md q-mb-md">
      <q-select v-model="formulaire.uniteId" :options="unites" option-value="id" option-label="nom"
                emit-value map-options label="Unité" dense outlined class="col-12 col-md-2"
                :rules="[(valeur) => !!valeur || 'Requis']" />
      <q-select v-model="formulaire.locataireId" :options="locataires" option-value="id" option-label="nom"
                emit-value map-options label="Locataire" dense outlined class="col-12 col-md-2"
                :rules="[(valeur) => !!valeur || 'Requis']" />
      <q-input v-model="formulaire.dateDebut" type="date" label="Début" stack-label dense outlined
               class="col-12 col-md-2" :rules="[(valeur) => !!valeur || 'Requis']" />
      <q-input v-model="formulaire.dateFin" type="date" label="Fin (facultatif)" stack-label dense outlined
               class="col-12 col-md-2" />
      <q-input v-model.number="formulaire.montant" type="number" label="Montant" dense outlined
               class="col-12 col-md-2" />
      <div class="col-12 col-md-2">
        <q-btn type="submit" color="primary" :label="idContratEnModification ? 'Enregistrer' : 'Ajouter'" />
        <q-btn v-if="idContratEnModification" flat label="Annuler" @click="viderFormulaire" />
      </div>
    </q-form>

    <q-table :rows="contrats" :columns="colonnes" row-key="id" flat bordered title="Contrats">
      <template v-slot:body-cell-actions="props">
        <q-td :props="props">
          <q-btn flat color="primary" label="Modifier" @click="commencerModification(props.row)" />
          <q-btn flat color="negative" label="Supprimer" @click="supprimerContrat(props.row)" />
        </q-td>
      </template>
    </q-table>
  </div>
</template>