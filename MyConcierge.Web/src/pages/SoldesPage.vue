<script setup>
import { ref, computed, onMounted } from 'vue'
import { api, afficherErreur } from '../api'
import { formaterMontant } from '../format'

const contrats = ref([])
const soldes = ref([]) // un solde par contrat : { contratId, loyerDu, totalPaye, solde }
const affichage = ref('contrat') // 'contrat' ou 'locataire'
const seulementEnRetard = ref(false)

const optionsAffichage = [
  { label: 'Par contrat', value: 'contrat' },
  { label: 'Par locataire', value: 'locataire' }
]

const colonnesParContrat = [
  { name: 'locataire', label: 'Locataire', field: 'locataire', align: 'left', sortable: true },
  { name: 'unite', label: 'Unité', field: 'unite', align: 'left', sortable: true },
  { name: 'montant', label: 'Loyer mensuel', field: 'montant', format: formaterMontant, align: 'right', sortable: true },
  { name: 'loyerDu', label: 'Dû', field: 'loyerDu', format: formaterMontant, align: 'right', sortable: true },
  { name: 'totalPaye', label: 'Payé', field: 'totalPaye', format: formaterMontant, align: 'right', sortable: true },
  { name: 'solde', label: 'Solde en retard', field: 'solde', format: formaterMontant, align: 'right', sortable: true },
  { name: 'statut', label: 'Statut', align: 'center' }
]

const colonnesParLocataire = [
  { name: 'locataire', label: 'Locataire', field: 'locataire', align: 'left', sortable: true },
  { name: 'nombreDeContrats', label: 'Contrats', field: 'nombreDeContrats', align: 'right', sortable: true },
  { name: 'loyerDu', label: 'Dû', field: 'loyerDu', format: formaterMontant, align: 'right', sortable: true },
  { name: 'totalPaye', label: 'Payé', field: 'totalPaye', format: formaterMontant, align: 'right', sortable: true },
  { name: 'solde', label: 'Solde en retard', field: 'solde', format: formaterMontant, align: 'right', sortable: true },
  { name: 'statut', label: 'Statut', align: 'center' }
]

// Le plus gros retard en premier
const triParDefaut = { sortBy: 'solde', descending: true, rowsPerPage: 10 }

async function chargerDonnees() {
  try {
    contrats.value = await api.get('/contrats')
    soldes.value = await api.get('/paiements/soldes')
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

onMounted(chargerDonnees)

// Une ligne par contrat : le contrat avec son solde
const lignesParContrat = computed(() =>
  contrats.value.map((contrat) => {
    const solde = soldes.value.find((soldeDuContrat) => soldeDuContrat.contratId === contrat.id)
    return {
      id: contrat.id,
      locataireId: contrat.locataireId,
      locataire: contrat.locataire?.nom,
      unite: contrat.unite?.nom,
      montant: contrat.montant,
      loyerDu: solde ? solde.loyerDu : 0,
      totalPaye: solde ? solde.totalPaye : 0,
      solde: solde ? solde.solde : 0
    }
  })
)

// Une ligne par locataire : on additionne ses contrats
const lignesParLocataire = computed(() => {
  const totauxParLocataire = {}
  for (const ligne of lignesParContrat.value) {
    if (!totauxParLocataire[ligne.locataireId]) {
      totauxParLocataire[ligne.locataireId] = {
        id: ligne.locataireId,
        locataire: ligne.locataire,
        nombreDeContrats: 0,
        loyerDu: 0,
        totalPaye: 0,
        solde: 0
      }
    }
    const totaux = totauxParLocataire[ligne.locataireId]
    totaux.nombreDeContrats += 1
    totaux.loyerDu += ligne.loyerDu
    totaux.totalPaye += ligne.totalPaye
    totaux.solde += ligne.solde
  }
  return Object.values(totauxParLocataire)
})

const lignesAffichees = computed(() => {
  const lignes = affichage.value === 'contrat' ? lignesParContrat.value : lignesParLocataire.value
  return seulementEnRetard.value ? lignes.filter((ligne) => ligne.solde > 0) : lignes
})

// Les totaux en haut de page (tous les contrats, peu importe le filtre)
const totalDu = computed(() => lignesParContrat.value.reduce((somme, ligne) => somme + ligne.loyerDu, 0))
const totalPaye = computed(() => lignesParContrat.value.reduce((somme, ligne) => somme + ligne.totalPaye, 0))
const totalEnRetard = computed(() => lignesParContrat.value.reduce((somme, ligne) => somme + ligne.solde, 0))
const nombreDeContratsEnRetard = computed(() => lignesParContrat.value.filter((ligne) => ligne.solde > 0).length)
</script>

<template>
  <div>
    <!-- Totaux -->
    <div class="row q-col-gutter-md q-mb-md">
      <div class="col-6 col-md-3">
        <q-card flat bordered class="q-pa-md">
          <div class="text-caption text-grey-7">Total dû</div>
          <div class="text-h6">{{ formaterMontant(totalDu) }}</div>
        </q-card>
      </div>
      <div class="col-6 col-md-3">
        <q-card flat bordered class="q-pa-md">
          <div class="text-caption text-grey-7">Total payé</div>
          <div class="text-h6">{{ formaterMontant(totalPaye) }}</div>
        </q-card>
      </div>
      <div class="col-6 col-md-3">
        <q-card flat bordered class="q-pa-md">
          <div class="text-caption text-grey-7">Solde en retard</div>
          <div class="text-h6" :class="totalEnRetard > 0 ? 'text-negative' : 'text-positive'">
            {{ formaterMontant(totalEnRetard) }}
          </div>
        </q-card>
      </div>
      <div class="col-6 col-md-3">
        <q-card flat bordered class="q-pa-md">
          <div class="text-caption text-grey-7">Contrats en retard</div>
          <div class="text-h6">{{ nombreDeContratsEnRetard }}</div>
        </q-card>
      </div>
    </div>

    <!-- Affichage -->
    <div class="row items-center q-mb-md">
      <q-btn-toggle v-model="affichage" :options="optionsAffichage" no-caps unelevated toggle-color="primary"
                    color="white" text-color="primary" />
      <q-space />
      <q-toggle v-model="seulementEnRetard" label="Seulement les retards" />
    </div>

    <q-table :rows="lignesAffichees" :columns="affichage === 'contrat' ? colonnesParContrat : colonnesParLocataire"
             row-key="id" flat bordered :pagination="triParDefaut" no-data-label="Aucun résultat.">
      <template v-slot:body-cell-statut="props">
        <q-td :props="props">
          <q-badge v-if="props.row.solde > 0" color="negative" label="En retard" />
          <q-badge v-else color="positive" label="À jour" />
        </q-td>
      </template>
    </q-table>
  </div>
</template>
