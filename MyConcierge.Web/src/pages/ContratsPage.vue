<script setup>
import { ref, nextTick, onMounted } from 'vue'
import { api, afficherErreur } from '../api'

const contrats = ref([])
const unites = ref([])
const utilisateurs = ref([])
const formulaire = ref(formulaireVide())
const formulaireRef = ref(null)

const colonnes = [
  { name: 'unite', label: 'Unité', field: (c) => c.unite?.nom, align: 'left' },
  { name: 'locataire', label: 'Locataire', field: (c) => c.locataire?.nom, align: 'left' },
  { name: 'dateDebut', label: 'Début', field: (c) => c.dateDebut.slice(0, 10), align: 'left' },
  { name: 'dateFin', label: 'Fin', field: (c) => (c.dateFin ? c.dateFin.slice(0, 10) : 'En cours'), align: 'left' },
  { name: 'montant', label: 'Montant', field: 'montant', align: 'right' },
  { name: 'actions', label: '', align: 'right' }
]

function formulaireVide() {
  return { uniteId: null, locataireId: null, dateDebut: '', dateFin: '', montant: 0 }
}

async function charger() {
  try {
    contrats.value = await api.get('/contrats')
    unites.value = await api.get('/unites')
    const tousLesUtilisateurs = await api.get('/utilisateurs')
    // Seuls les locataires peuvent signer un contrat
    utilisateurs.value = tousLesUtilisateurs.filter((u) => u.referenceType?.nom === 'Locataire')
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

async function ajouter() {
  try {
    await api.post('/contrats', {
      ...formulaire.value,
      dateFin: formulaire.value.dateFin || null // vide = contrat sans fin
    })
    formulaire.value = formulaireVide()
    await nextTick()
    formulaireRef.value.resetValidation() // évite les champs rouges après le vidage
    await charger()
  } catch (erreur) {
    afficherErreur(erreur) // ex : « Cette unité est déjà louée sur cette période. »
  }
}

async function supprimer(contrat) {
  if (!confirm('Supprimer ce contrat ?')) return
  try {
    await api.delete('/contrats/' + contrat.id)
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
      <q-select v-model="formulaire.uniteId" :options="unites" option-value="id" option-label="nom"
                emit-value map-options label="Unité" dense outlined class="col-12 col-md-2"
                :rules="[(v) => !!v || 'Requis']" />
      <q-select v-model="formulaire.locataireId" :options="utilisateurs" option-value="id" option-label="nom"
                emit-value map-options label="Locataire" dense outlined class="col-12 col-md-2"
                :rules="[(v) => !!v || 'Requis']" />
      <q-input v-model="formulaire.dateDebut" type="date" label="Début" stack-label dense outlined
               class="col-12 col-md-2" :rules="[(v) => !!v || 'Requis']" />
      <q-input v-model="formulaire.dateFin" type="date" label="Fin (facultatif)" stack-label dense outlined
               class="col-12 col-md-2" />
      <q-input v-model.number="formulaire.montant" type="number" label="Montant" dense outlined
               class="col-12 col-md-2" />
      <div class="col-12 col-md-2">
        <q-btn type="submit" color="primary" label="Ajouter" />
      </div>
    </q-form>

    <q-table :rows="contrats" :columns="colonnes" row-key="id" flat bordered title="Contrats">
      <template v-slot:body-cell-actions="props">
        <q-td :props="props">
          <q-btn flat color="negative" label="Supprimer" @click="supprimer(props.row)" />
        </q-td>
      </template>
    </q-table>
  </div>
</template>
