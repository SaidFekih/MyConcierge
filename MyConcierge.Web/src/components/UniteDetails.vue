<script setup>
import { ref, computed, onMounted } from 'vue'
import { api, afficherErreur } from '../api'

const props = defineProps({
  unite: Object,
  typesUnite: Array,
  proprietaires: Array,
  locataires: Array
})

// donnees-modifiees : la fenêtre reste ouverte, la page recharge l'unité
// unite-supprimee : la page ferme la fenêtre
const emit = defineEmits(['donnees-modifiees', 'unite-supprimee'])

const contratsDeLUnite = ref([])

const modificationUniteOuverte = ref(false)
const formulaireUnite = ref({})

const terminaisonContratOuverte = ref(false)
const dateFinChoisie = ref('')

const ajoutContratOuvert = ref(false)
const formulaireContrat = ref({})

const colonnesHistorique = [
  { name: 'locataire', label: 'Locataire', field: (contrat) => contrat.locataire?.nom, align: 'left' },
  { name: 'dateDebut', label: 'Début', field: (contrat) => contrat.dateDebut.slice(0, 10), align: 'left' },
  { name: 'dateFin', label: 'Fin', field: (contrat) => (contrat.dateFin ? contrat.dateFin.slice(0, 10) : 'En cours'), align: 'left' },
  { name: 'montant', label: 'Montant', field: 'montant', align: 'right' }
]

function dateDuJour() {
  return new Date().toLocaleDateString('en-CA') // AAAA-MM-JJ, heure locale
}

// Contrat en cours : commencé, et pas encore terminé
const contratEnCours = computed(() => {
  const aujourdhui = dateDuJour()
  return contratsDeLUnite.value.find((contrat) =>
    contrat.dateDebut.slice(0, 10) <= aujourdhui &&
    (!contrat.dateFin || contrat.dateFin.slice(0, 10) >= aujourdhui)
  )
})

async function chargerContrats() {
  try {
    const tousLesContrats = await api.get('/contrats')
    contratsDeLUnite.value = tousLesContrats
      .filter((contrat) => contrat.uniteId === props.unite.id)
      .sort((contratA, contratB) => contratB.dateDebut.localeCompare(contratA.dateDebut)) // le plus récent en premier
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

// Après une action sur un contrat : recharge l'historique et prévient la page (EstLouee a pu changer)
async function apresChangementDeContrat() {
  await chargerContrats()
  emit('donnees-modifiees')
}

// --- Modifier l'unité ---

function commencerModificationUnite() {
  formulaireUnite.value = {
    nom: props.unite.nom,
    referenceTypeId: props.unite.referenceTypeId,
    proprietaireId: props.unite.proprietaireId,
    prix: props.unite.prix,
    estLouee: props.unite.estLouee // conservé tel quel : ce sont les contrats qui le changent
  }
  modificationUniteOuverte.value = true
}

async function enregistrerUnite() {
  try {
    await api.put('/unites/' + props.unite.id, { id: props.unite.id, ...formulaireUnite.value })
    modificationUniteOuverte.value = false
    emit('donnees-modifiees')
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

// --- Supprimer l'unité ---

async function supprimerUnite() {
  if (!confirm('Supprimer l\'unité « ' + props.unite.nom + ' » ?')) return
  try {
    await api.delete('/unites/' + props.unite.id)
    emit('unite-supprimee')
  } catch (erreur) {
    afficherErreur(erreur) // ex : « Cette unité a un contrat en cours ou à venir… »
  }
}

// --- Terminer le contrat en cours ---

function commencerTerminaisonContrat() {
  dateFinChoisie.value = dateDuJour()
  terminaisonContratOuverte.value = true
}

async function terminerContrat() {
  const contrat = contratEnCours.value
  try {
    await api.put('/contrats/' + contrat.id, {
      id: contrat.id,
      uniteId: contrat.uniteId,
      locataireId: contrat.locataireId,
      dateDebut: contrat.dateDebut.slice(0, 10),
      dateFin: dateFinChoisie.value,
      montant: contrat.montant
    })
    terminaisonContratOuverte.value = false
    await apresChangementDeContrat()
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

// --- Ajouter un contrat ---

function commencerAjoutContrat() {
  formulaireContrat.value = {
    locataireId: null,
    dateDebut: dateDuJour(),
    dateFin: '',
    montant: props.unite.prix // proposé : le prix de l'unité
  }
  ajoutContratOuvert.value = true
}

async function ajouterContrat() {
  try {
    await api.post('/contrats', {
      uniteId: props.unite.id,
      locataireId: formulaireContrat.value.locataireId,
      dateDebut: formulaireContrat.value.dateDebut,
      dateFin: formulaireContrat.value.dateFin || null, // vide = contrat sans fin
      montant: formulaireContrat.value.montant
    })
    ajoutContratOuvert.value = false
    await apresChangementDeContrat()
  } catch (erreur) {
    afficherErreur(erreur) // ex : « Cette unité est déjà louée sur cette période. »
  }
}

onMounted(chargerContrats)
</script>

<template>
  <q-card style="min-width: 700px; max-width: 90vw">
    <q-card-section class="row items-center">
      <div class="text-h6">{{ unite.nom }}</div>
      <q-space />
      <q-btn icon="close" flat round dense v-close-popup />
    </q-card-section>

    <!-- Détails de l'unité -->
    <q-card-section>
      <div class="text-subtitle2">Détails</div>

      <q-form v-if="modificationUniteOuverte" @submit="enregistrerUnite" class="row q-col-gutter-md">
        <q-input v-model="formulaireUnite.nom" label="Nom" dense outlined class="col-12 col-md-3"
                 :rules="[(valeur) => !!valeur || 'Requis']" />
        <q-select v-model="formulaireUnite.referenceTypeId" :options="typesUnite" option-value="id" option-label="nom"
                  emit-value map-options label="Type" dense outlined class="col-12 col-md-3"
                  :rules="[(valeur) => !!valeur || 'Requis']" />
        <q-select v-model="formulaireUnite.proprietaireId" :options="proprietaires" option-value="id" option-label="nom"
                  emit-value map-options label="Propriétaire" dense outlined class="col-12 col-md-3"
                  :rules="[(valeur) => !!valeur || 'Requis']" />
        <q-input v-model.number="formulaireUnite.prix" type="number" label="Prix" dense outlined class="col-12 col-md-3" />
        <div class="col-12">
          <q-btn type="submit" color="primary" label="Enregistrer" />
          <q-btn flat label="Annuler" @click="modificationUniteOuverte = false" />
        </div>
      </q-form>

      <div v-else>
        <div>Type : {{ unite.referenceType?.nom }}</div>
        <div>Propriétaire : {{ unite.proprietaire?.nom }}</div>
        <div>Prix : {{ unite.prix }}</div>
        <div>Louée : {{ unite.estLouee ? 'Oui' : 'Non' }}</div>
        <q-btn flat color="primary" label="Modifier l'unité" @click="commencerModificationUnite" />
        <q-btn flat color="negative" label="Supprimer l'unité" @click="supprimerUnite" />
      </div>
    </q-card-section>

    <!-- Contrat en cours -->
    <q-card-section>
      <div class="text-subtitle2">Contrat en cours</div>

      <div v-if="contratEnCours">
        {{ contratEnCours.locataire?.nom }} — depuis le {{ contratEnCours.dateDebut.slice(0, 10) }}
        — {{ contratEnCours.montant }}

        <div v-if="terminaisonContratOuverte" class="row q-col-gutter-md items-start q-mt-none">
          <q-input v-model="dateFinChoisie" type="date" label="Date de fin" stack-label dense outlined
                   hint="Le contrat reste en cours jusqu'à cette date incluse." class="col-12 col-md-5" />
          <div class="col-12 col-md-7">
            <q-btn color="primary" label="Confirmer" @click="terminerContrat" />
            <q-btn flat label="Annuler" @click="terminaisonContratOuverte = false" />
          </div>
        </div>
        <div v-else>
          <q-btn flat color="primary" label="Terminer le contrat" @click="commencerTerminaisonContrat" />
        </div>
      </div>

      <div v-else>Aucun contrat en cours.</div>
    </q-card-section>

    <!-- Ajouter un contrat -->
    <q-card-section>
      <q-form v-if="ajoutContratOuvert" @submit="ajouterContrat" class="row q-col-gutter-md">
        <q-select v-model="formulaireContrat.locataireId" :options="locataires" option-value="id" option-label="nom"
                  emit-value map-options label="Locataire" dense outlined class="col-12 col-md-3"
                  :rules="[(valeur) => !!valeur || 'Requis']" />
        <q-input v-model="formulaireContrat.dateDebut" type="date" label="Début" stack-label dense outlined
                 class="col-12 col-md-3" :rules="[(valeur) => !!valeur || 'Requis']" />
        <q-input v-model="formulaireContrat.dateFin" type="date" label="Fin (facultatif)" stack-label dense outlined
                 class="col-12 col-md-3" />
        <q-input v-model.number="formulaireContrat.montant" type="number" label="Montant" dense outlined
                 class="col-12 col-md-3" />
        <div class="col-12">
          <q-btn type="submit" color="primary" label="Ajouter le contrat" />
          <q-btn flat label="Annuler" @click="ajoutContratOuvert = false" />
        </div>
      </q-form>
      <q-btn v-else flat color="primary" label="+ Ajouter un contrat" @click="commencerAjoutContrat" />
    </q-card-section>

    <!-- Historique -->
    <q-card-section>
      <q-table title="Historique des contrats" :rows="contratsDeLUnite" :columns="colonnesHistorique"
               row-key="id" flat bordered dense />
    </q-card-section>
  </q-card>
</template>
