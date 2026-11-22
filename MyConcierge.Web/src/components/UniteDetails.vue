<script setup>
import { ref, computed } from 'vue'
import { api, afficherErreur, afficherSucces } from '../api'
import { formaterDate, formaterMontant } from '../format'
import { dateDuJour, estAVenir, estEnCours } from '../contrats'

const props = defineProps({
  unite: Object,
  contrats: Array, // tous les contrats : la page les recharge après chaque action
  typesUnite: Array,
  proprietaires: Array,
  locataires: Array
})

// donnees-modifiees : le panneau reste ouvert, la page recharge les données
// unite-supprimee : la page ferme le panneau
const emit = defineEmits(['donnees-modifiees', 'unite-supprimee'])

const modificationUniteOuverte = ref(false)
const formulaireUnite = ref({})

const terminaisonContratOuverte = ref(false)
const dateFinChoisie = ref('')

const ajoutContratOuvert = ref(false)
const formulaireContrat = ref({})

const colonnesHistorique = [
  { name: 'locataire', label: 'Locataire', field: (contrat) => contrat.locataire?.nom, align: 'left' },
  { name: 'dateDebut', label: 'Début', field: (contrat) => formaterDate(contrat.dateDebut), align: 'left' },
  { name: 'dateFin', label: 'Fin', field: (contrat) => (contrat.dateFin ? formaterDate(contrat.dateFin) : 'Sans fin'), align: 'left' },
  { name: 'montant', label: 'Montant', field: (contrat) => formaterMontant(contrat.montant), align: 'right' },
  { name: 'statut', label: 'Statut', align: 'center' }
]

const contratsDeLUnite = computed(() =>
  props.contrats
    .filter((contrat) => contrat.uniteId === props.unite.id)
    .sort((contratA, contratB) => contratB.dateDebut.localeCompare(contratA.dateDebut)) // le plus récent en premier
)

const contratEnCours = computed(() => contratsDeLUnite.value.find(estEnCours))

// Loyer actuel moins loyer cible (null si l'unité n'a pas de contrat en cours)
const ecartDeLoyer = computed(() =>
  contratEnCours.value ? contratEnCours.value.montant - props.unite.prix : null
)

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

// --- Dupliquer l'unité (sans ses contrats) ---

async function dupliquerUnite() {
  const nomDeLaCopie = props.unite.nom + ' (copie)'
  try {
    await api.post('/unites', {
      nom: nomDeLaCopie,
      referenceTypeId: props.unite.referenceTypeId,
      proprietaireId: props.unite.proprietaireId,
      prix: props.unite.prix,
      estLouee: false
    })
    afficherSucces('Unité dupliquée : « ' + nomDeLaCopie + ' »')
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
    emit('donnees-modifiees')
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
    emit('donnees-modifiees')
  } catch (erreur) {
    afficherErreur(erreur) // ex : « Cette unité est déjà louée sur cette période. »
  }
}
</script>

<template>
  <q-card class="column no-wrap" style="width: 520px; max-width: 95vw; height: 100%">
    <!-- En-tête -->
    <div class="row items-center no-wrap q-pa-md">
      <div class="text-h6 ellipsis">{{ unite.nom }}</div>
      <q-badge v-if="contratEnCours" color="positive" label="Louée" class="q-ml-md q-pa-xs" />
      <q-badge v-else color="grey-6" label="Vacante" class="q-ml-md q-pa-xs" />
      <q-space />
      <q-btn icon="close" flat round dense v-close-popup />
    </div>
    <q-separator />

    <div class="col scroll q-pa-md">
      <!-- Actions sur l'unité -->
      <div class="row q-col-gutter-sm q-mb-md">
        <div class="col-4">
          <q-btn outline no-caps color="primary" icon="edit" label="Modifier" class="full-width"
                 @click="commencerModificationUnite" />
        </div>
        <div class="col-4">
          <q-btn outline no-caps color="primary" icon="content_copy" label="Dupliquer" class="full-width"
                 @click="dupliquerUnite" />
        </div>
        <div class="col-4">
          <q-btn outline no-caps color="negative" icon="delete" label="Supprimer" class="full-width"
                 @click="supprimerUnite" />
        </div>
      </div>

      <!-- Formulaire de modification -->
      <q-card v-if="modificationUniteOuverte" flat bordered class="q-pa-md q-mb-md">
        <q-form @submit="enregistrerUnite" class="row q-col-gutter-md">
          <q-input v-model="formulaireUnite.nom" label="Nom" dense outlined class="col-12 col-sm-6"
                   :rules="[(valeur) => !!valeur || 'Requis']" />
          <q-select v-model="formulaireUnite.referenceTypeId" :options="typesUnite" option-value="id" option-label="nom"
                    emit-value map-options label="Type" dense outlined class="col-12 col-sm-6"
                    :rules="[(valeur) => !!valeur || 'Requis']" />
          <q-select v-model="formulaireUnite.proprietaireId" :options="proprietaires" option-value="id" option-label="nom"
                    emit-value map-options label="Propriétaire" dense outlined class="col-12 col-sm-6"
                    :rules="[(valeur) => !!valeur || 'Requis']" />
          <q-input v-model.number="formulaireUnite.prix" type="number" label="Prix" dense outlined class="col-12 col-sm-6" />
          <div class="col-12 text-right">
            <q-btn flat no-caps label="Annuler" class="q-mr-sm" @click="modificationUniteOuverte = false" />
            <q-btn type="submit" unelevated no-caps color="primary" label="Enregistrer" />
          </div>
        </q-form>
      </q-card>

      <!-- Cartes : loyer cible, loyer actuel, écart -->
      <div class="row q-col-gutter-sm q-mb-md">
        <div class="col-4">
          <q-card flat bordered class="q-pa-sm">
            <div class="text-caption text-grey-7">Loyer cible</div>
            <div class="text-subtitle1 text-weight-medium">{{ formaterMontant(unite.prix) }}</div>
          </q-card>
        </div>
        <div class="col-4">
          <q-card flat bordered class="q-pa-sm">
            <div class="text-caption text-grey-7">Loyer actuel</div>
            <div class="text-subtitle1 text-weight-medium">
              {{ contratEnCours ? formaterMontant(contratEnCours.montant) : '—' }}
            </div>
          </q-card>
        </div>
        <div class="col-4">
          <q-card flat bordered class="q-pa-sm">
            <div class="text-caption text-grey-7">Écart</div>
            <div v-if="ecartDeLoyer !== null" class="text-subtitle1 text-weight-medium"
                 :class="{ 'text-negative': ecartDeLoyer < 0, 'text-positive': ecartDeLoyer > 0 }">
              {{ ecartDeLoyer > 0 ? '+' : '' }}{{ formaterMontant(ecartDeLoyer) }}
            </div>
            <div v-else class="text-subtitle1 text-weight-medium">—</div>
          </q-card>
        </div>
      </div>

      <!-- Type et propriétaire -->
      <div v-if="!modificationUniteOuverte" class="row q-col-gutter-md q-mb-md">
        <div class="col-6">
          <div class="text-caption text-grey-7">Type</div>
          <div>{{ unite.referenceType?.nom }}</div>
        </div>
        <div class="col-6">
          <div class="text-caption text-grey-7">Propriétaire</div>
          <div>{{ unite.proprietaire?.nom }}</div>
        </div>
      </div>

      <q-separator class="q-mb-md" />

      <!-- Contrat en cours -->
      <div class="text-subtitle2 q-mb-sm">Contrat en cours</div>

      <q-card v-if="contratEnCours" flat bordered class="q-pa-md">
        <div class="row items-center no-wrap">
          <q-icon name="person" size="sm" color="primary" class="q-mr-sm" />
          <div class="text-subtitle1 text-weight-medium ellipsis">{{ contratEnCours.locataire?.nom }}</div>
          <q-space />
          <div class="text-subtitle1">{{ formaterMontant(contratEnCours.montant) }}</div>
        </div>
        <div class="text-caption text-grey-7 q-mt-xs">
          Depuis le {{ formaterDate(contratEnCours.dateDebut) }} ·
          {{ contratEnCours.dateFin ? 'jusqu\'au ' + formaterDate(contratEnCours.dateFin) : 'sans date de fin' }}
        </div>

        <div v-if="terminaisonContratOuverte" class="row q-col-gutter-md items-start q-mt-xs">
          <q-input v-model="dateFinChoisie" type="date" label="Date de fin" stack-label dense outlined
                   hint="Le contrat reste en cours jusqu'à cette date incluse." class="col-12" />
          <div class="col-12 text-right">
            <q-btn flat no-caps label="Annuler" class="q-mr-sm" @click="terminaisonContratOuverte = false" />
            <q-btn unelevated no-caps color="primary" label="Confirmer" @click="terminerContrat" />
          </div>
        </div>
        <div v-else class="text-right q-mt-sm">
          <q-btn outline dense no-caps color="primary" icon="event_busy" label="Terminer le contrat" class="q-px-sm"
                 @click="commencerTerminaisonContrat" />
        </div>
      </q-card>

      <div v-else class="row items-center text-grey-7">
        <q-icon name="info" class="q-mr-sm" />
        Aucun contrat en cours.
      </div>

      <!-- Ajouter un contrat -->
      <q-card v-if="ajoutContratOuvert" flat bordered class="q-pa-md q-mt-md">
        <div class="text-subtitle2 q-mb-sm">Nouveau contrat</div>
        <q-form @submit="ajouterContrat" class="row q-col-gutter-md">
          <q-select v-model="formulaireContrat.locataireId" :options="locataires" option-value="id" option-label="nom"
                    emit-value map-options label="Locataire" dense outlined class="col-12"
                    :rules="[(valeur) => !!valeur || 'Requis']" />
          <q-input v-model="formulaireContrat.dateDebut" type="date" label="Début" stack-label dense outlined
                   class="col-12 col-sm-6" :rules="[(valeur) => !!valeur || 'Requis']" />
          <q-input v-model="formulaireContrat.dateFin" type="date" label="Fin (facultatif)" stack-label dense outlined
                   class="col-12 col-sm-6" />
          <q-input v-model.number="formulaireContrat.montant" type="number" label="Montant" dense outlined
                   class="col-12" />
          <div class="col-12 text-right">
            <q-btn flat no-caps label="Annuler" class="q-mr-sm" @click="ajoutContratOuvert = false" />
            <q-btn type="submit" unelevated no-caps color="primary" label="Ajouter le contrat" />
          </div>
        </q-form>
      </q-card>
      <q-btn v-else outline no-caps color="primary" icon="add" label="Ajouter un contrat" class="full-width q-mt-md"
             @click="commencerAjoutContrat" />

      <q-separator class="q-my-md" />

      <!-- Historique -->
      <div class="text-subtitle2 q-mb-sm">Historique des contrats</div>
      <q-table :rows="contratsDeLUnite" :columns="colonnesHistorique" row-key="id" flat bordered dense
               :pagination="{ rowsPerPage: 0 }" hide-bottom no-data-label="Aucun contrat pour cette unité.">
        <template v-slot:body-cell-statut="props">
          <q-td :props="props">
            <q-badge v-if="estEnCours(props.row)" color="positive" label="En cours" />
            <q-badge v-else-if="estAVenir(props.row)" color="info" label="À venir" />
            <q-badge v-else color="grey-6" label="Terminé" />
          </q-td>
        </template>
      </q-table>
    </div>
  </q-card>
</template>
