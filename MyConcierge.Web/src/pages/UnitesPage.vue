<script setup>
import { ref, computed, onMounted } from 'vue'
import { api, afficherErreur } from '../api'
import UniteDetails from '../components/UniteDetails.vue'
import { formaterMontant } from '../format'
import { estEnCours } from '../contrats'

const unites = ref([])
const contrats = ref([])
const paiements = ref([])
const soldes = ref([]) // un solde par contrat : { contratId, loyerDu, totalPaye, solde }
const typesUnite = ref([])
const proprietaires = ref([])
const locataires = ref([])

// Recherche et filtres (0 = Tous)
const recherche = ref('')
const filtreTypeId = ref(0)
const filtreProprietaireId = ref(0)

const pagination = ref({ rowsPerPage: 10 })

const nouvelleUniteOuverte = ref(false)
const formulaireNouvelleUnite = ref(creerFormulaireVide())

const uniteSelectionnee = ref(null)
const panneauOuvert = ref(false)

const colonnes = [
  { name: 'nom', label: 'Unité', field: 'nom', align: 'left', sortable: true },
  { name: 'type', label: 'Type', field: (unite) => unite.referenceType?.nom, align: 'left', sortable: true },
  { name: 'proprietaire', label: 'Propriétaire', field: (unite) => unite.proprietaire?.nom, align: 'left', sortable: true },
  { name: 'loyer', label: 'Loyer', field: (unite) => texteLoyer(unite), align: 'right' },
  { name: 'statut', label: 'Statut', align: 'center' }
]

const optionsFiltreType = computed(() => [{ id: 0, nom: 'Tous' }, ...typesUnite.value])
const optionsFiltreProprietaire = computed(() => [{ id: 0, nom: 'Tous' }, ...proprietaires.value])

const unitesFiltrees = computed(() =>
  unites.value.filter((unite) =>
    unite.nom.toLowerCase().includes(recherche.value.toLowerCase()) &&
    (filtreTypeId.value === 0 || unite.referenceTypeId === filtreTypeId.value) &&
    (filtreProprietaireId.value === 0 || unite.proprietaireId === filtreProprietaireId.value)
  )
)

function creerFormulaireVide() {
  return { nom: '', referenceTypeId: null, proprietaireId: null, prix: 0, estLouee: false }
}

function contratEnCoursDe(unite) {
  return contrats.value.find((contrat) => contrat.uniteId === unite.id && estEnCours(contrat))
}

// Louée = a un contrat en cours aujourd'hui (calculé avec les dates, donc toujours à jour)
function estLouee(unite) {
  return contratEnCoursDe(unite) !== undefined
}

// Loyer actuel / loyer cible, ou seulement le loyer cible si l'unité est vacante
function texteLoyer(unite) {
  const contrat = contratEnCoursDe(unite)
  if (!contrat) return formaterMontant(unite.prix)
  return formaterMontant(contrat.montant) + ' / ' + formaterMontant(unite.prix)
}

async function chargerDonnees() {
  try {
    unites.value = await api.get('/unites')
    contrats.value = await api.get('/contrats')
    paiements.value = await api.get('/paiements')
    soldes.value = await api.get('/paiements/soldes')
    const tousLesTypes = await api.get('/referencetypes')
    typesUnite.value = tousLesTypes.filter((type) => type.categorie === 'Unite')
    const utilisateurs = await api.get('/utilisateurs')
    // Seuls les propriétaires peuvent posséder une unité, seuls les locataires peuvent louer
    proprietaires.value = utilisateurs.filter((utilisateur) => utilisateur.referenceType?.nom === 'Propriétaire')
    locataires.value = utilisateurs.filter((utilisateur) => utilisateur.referenceType?.nom === 'Locataire')
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

// --- Nouvelle unité ---

function ouvrirNouvelleUnite() {
  formulaireNouvelleUnite.value = creerFormulaireVide()
  nouvelleUniteOuverte.value = true
}

async function ajouterUnite() {
  try {
    await api.post('/unites', formulaireNouvelleUnite.value)
    nouvelleUniteOuverte.value = false
    await chargerDonnees()
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

// --- Panneau de l'unité ---

// Un clic sur une ligne ouvre le panneau de l'unité
function ouvrirPanneau(evenement, unite) {
  uniteSelectionnee.value = unite
  panneauOuvert.value = true
}

// Après une action dans le panneau : recharge les données et remet à jour l'unité affichée
async function rafraichirPanneau() {
  await chargerDonnees()
  uniteSelectionnee.value = unites.value.find((unite) => unite.id === uniteSelectionnee.value.id)
}

// Après la suppression de l'unité : ferme le panneau et recharge la liste
async function fermerPanneauApresSuppression() {
  panneauOuvert.value = false
  await chargerDonnees()
}

onMounted(chargerDonnees)
</script>

<template>
  <div>
    <!-- Recherche, filtres et création -->
    <div class="row q-col-gutter-md items-center q-mb-md">
      <q-input v-model="recherche" placeholder="Rechercher par nom..." dense outlined clearable bg-color="white"
               class="col-12 col-md-4">
        <template v-slot:prepend>
          <q-icon name="search" />
        </template>
      </q-input>
      <q-select v-model="filtreTypeId" :options="optionsFiltreType" option-value="id" option-label="nom"
                emit-value map-options label="Type" dense outlined bg-color="white" class="col-6 col-md-2" />
      <q-select v-model="filtreProprietaireId" :options="optionsFiltreProprietaire" option-value="id" option-label="nom"
                emit-value map-options label="Propriétaire" dense outlined bg-color="white" class="col-6 col-md-2" />
      <q-space />
      <div class="col-auto">
        <q-btn unelevated color="primary" icon="add" label="Nouvelle unité" no-caps @click="ouvrirNouvelleUnite" />
      </div>
    </div>

    <q-table :rows="unitesFiltrees" :columns="colonnes" row-key="id" flat bordered title="Unités"
             v-model:pagination="pagination" :rows-per-page-options="[10, 25, 50]"
             no-data-label="Aucune unité ne correspond à la recherche." @row-click="ouvrirPanneau">
      <template v-slot:body-cell-statut="props">
        <q-td :props="props">
          <q-badge v-if="estLouee(props.row)" color="positive" label="Louée" />
          <q-badge v-else color="grey-6" label="Vacante" />
        </q-td>
      </template>
    </q-table>

    <!-- Panneau latéral de l'unité (la page reste utilisable à côté) -->
    <q-dialog v-model="panneauOuvert" position="right" full-height seamless>
      <UniteDetails v-if="uniteSelectionnee" :key="uniteSelectionnee.id" :unite="uniteSelectionnee"
                    :contrats="contrats" :paiements="paiements" :soldes="soldes"
                    :types-unite="typesUnite" :proprietaires="proprietaires"
                    :locataires="locataires" @donnees-modifiees="rafraichirPanneau"
                    @unite-supprimee="fermerPanneauApresSuppression" />
    </q-dialog>

    <!-- Fenêtre « Nouvelle unité » -->
    <q-dialog v-model="nouvelleUniteOuverte">
      <q-card style="width: 520px; max-width: 95vw">
        <q-card-section class="row items-center">
          <div class="text-h6">Nouvelle unité</div>
          <q-space />
          <q-btn icon="close" flat round dense v-close-popup />
        </q-card-section>
        <q-card-section>
          <q-form @submit="ajouterUnite" class="column q-gutter-md">
            <q-input v-model="formulaireNouvelleUnite.nom" label="Nom" dense outlined
                     :rules="[(valeur) => !!valeur || 'Requis']" />
            <q-select v-model="formulaireNouvelleUnite.referenceTypeId" :options="typesUnite" option-value="id"
                      option-label="nom" emit-value map-options label="Type" dense outlined
                      :rules="[(valeur) => !!valeur || 'Requis']" />
            <q-select v-model="formulaireNouvelleUnite.proprietaireId" :options="proprietaires" option-value="id"
                      option-label="nom" emit-value map-options label="Propriétaire" dense outlined
                      :rules="[(valeur) => !!valeur || 'Requis']" />
            <q-input v-model.number="formulaireNouvelleUnite.prix" type="number" label="Prix" dense outlined />
            <div class="text-right">
              <q-btn flat no-caps label="Annuler" class="q-mr-sm" v-close-popup />
              <q-btn type="submit" unelevated no-caps color="primary" label="Créer l'unité" />
            </div>
          </q-form>
        </q-card-section>
      </q-card>
    </q-dialog>
  </div>
</template>
