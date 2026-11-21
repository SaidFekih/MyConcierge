<script setup>
import { ref, nextTick, onMounted } from 'vue'
import { api, afficherErreur } from '../api'

const utilisateurs = ref([])
const typesUtilisateur = ref([])
const formulaire = ref(creerFormulaireVide())
const formulaireRef = ref(null)
const idUtilisateurEnModification = ref(null) // null = on ajoute un nouvel utilisateur

const colonnes = [
  { name: 'nom', label: 'Nom', field: 'nom', align: 'left' },
  { name: 'email', label: 'Email', field: 'email', align: 'left' },
  { name: 'telephone', label: 'Téléphone', field: 'telephone', align: 'left' },
  { name: 'type', label: 'Type', field: (utilisateur) => utilisateur.referenceType?.nom, align: 'left' },
  { name: 'actions', label: '', align: 'right' }
]

function creerFormulaireVide() {
  return { nom: '', email: '', telephone: '', referenceTypeId: null }
}

async function chargerDonnees() {
  try {
    utilisateurs.value = await api.get('/utilisateurs')
    const tousLesTypes = await api.get('/referencetypes')
    typesUtilisateur.value = tousLesTypes.filter((type) => type.categorie === 'Utilisateur')
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

async function viderFormulaire() {
  formulaire.value = creerFormulaireVide()
  idUtilisateurEnModification.value = null
  await nextTick()
  formulaireRef.value.resetValidation()
}

// Remplit le formulaire avec l'utilisateur choisi
function commencerModification(utilisateur) {
  idUtilisateurEnModification.value = utilisateur.id
  formulaire.value = {
    nom: utilisateur.nom,
    email: utilisateur.email,
    telephone: utilisateur.telephone,
    referenceTypeId: utilisateur.referenceTypeId
  }
}

async function enregistrerUtilisateur() {
  try {
    if (idUtilisateurEnModification.value) {
      await api.put('/utilisateurs/' + idUtilisateurEnModification.value, {
        id: idUtilisateurEnModification.value,
        ...formulaire.value
      })
    } else {
      await api.post('/utilisateurs', formulaire.value)
    }
    await viderFormulaire()
    await chargerDonnees()
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

async function supprimerUtilisateur(utilisateur) {
  if (!confirm('Supprimer « ' + utilisateur.nom + ' » ?')) return
  try {
    await api.delete('/utilisateurs/' + utilisateur.id)
    await chargerDonnees()
  } catch (erreur) {
    afficherErreur(erreur)
  }
}

onMounted(chargerDonnees)
</script>

<template>
  <div>
    <q-form ref="formulaireRef" @submit="enregistrerUtilisateur" class="row q-col-gutter-md q-mb-md">
      <q-input v-model="formulaire.nom" label="Nom" dense outlined class="col-12 col-md-3"
               :rules="[(valeur) => !!valeur || 'Requis']" />
      <q-input v-model="formulaire.email" type="email" label="Email" dense outlined class="col-12 col-md-3"
               :rules="[(valeur) => !!valeur || 'Requis']" />
      <q-input v-model="formulaire.telephone" label="Téléphone" dense outlined class="col-12 col-md-2" />
      <q-select v-model="formulaire.referenceTypeId" :options="typesUtilisateur" option-value="id" option-label="nom"
                emit-value map-options label="Type" dense outlined class="col-12 col-md-2"
                :rules="[(valeur) => !!valeur || 'Requis']" />
      <div class="col-12 col-md-2">
        <q-btn type="submit" color="primary" :label="idUtilisateurEnModification ? 'Enregistrer' : 'Ajouter'" />
        <q-btn v-if="idUtilisateurEnModification" flat label="Annuler" @click="viderFormulaire" />
      </div>
    </q-form>

    <q-table :rows="utilisateurs" :columns="colonnes" row-key="id" flat bordered title="Utilisateurs">
      <template v-slot:body-cell-actions="props">
        <q-td :props="props">
          <q-btn flat color="primary" label="Modifier" @click="commencerModification(props.row)" />
          <q-btn flat color="negative" label="Supprimer" @click="supprimerUtilisateur(props.row)" />
        </q-td>
      </template>
    </q-table>
  </div>
</template>