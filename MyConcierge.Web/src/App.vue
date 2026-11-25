<script setup>
import { ref } from 'vue'
import UnitesPage from './pages/UnitesPage.vue'
import ContratsPage from './pages/ContratsPage.vue'
import SoldesPage from './pages/SoldesPage.vue'
import UtilisateursPage from './pages/UtilisateursPage.vue'
import GestionnairesPage from './pages/GestionnairesPage.vue'
import ConnexionPage from './pages/ConnexionPage.vue'
import { session, fermerSession } from './session'
import ReferenceTypesPage from './pages/ReferenceTypesPage.vue'

const onglet = ref('unites')
</script>

<template>
  <!-- Pas de jeton : page de connexion seulement -->
  <ConnexionPage v-if="!session.jeton" />

  <q-layout v-else view="hHh lpR fFf" class="bg-grey-2">
    <q-header bordered class="bg-white text-grey-9">
      <q-toolbar>
        <q-icon name="apartment" color="primary" size="sm" class="q-mr-sm" />
        <q-toolbar-title class="text-weight-medium">MyConcierge</q-toolbar-title>
        <span class="text-grey-7 q-mr-sm">{{ session.nom }}</span>
        <q-btn flat dense icon="logout" label="Déconnexion" @click="fermerSession" />
      </q-toolbar>
      <q-tabs v-model="onglet" align="left" active-color="primary" indicator-color="primary">
        <q-tab name="unites" label="Unités" />
        <q-tab name="utilisateurs" label="Utilisateurs" />
        <q-tab name="contrats" label="Contrats" />
        <q-tab name="soldes" label="Paiements et soldes" />
        <q-tab name="types" label="Types" />
        <q-tab name="gestionnaires" label="Gestionnaires" />
      </q-tabs>
    </q-header>

    <q-page-container>
      <q-page padding>
        <!-- v-if : la page se recharge à chaque changement d'onglet -->
        <UnitesPage v-if="onglet === 'unites'" />
        <UtilisateursPage v-else-if="onglet === 'utilisateurs'" />
        <ContratsPage v-else-if="onglet === 'contrats'" />
        <SoldesPage v-else-if="onglet === 'soldes'" />
        <GestionnairesPage v-else-if="onglet === 'gestionnaires'" />
        <ReferenceTypesPage v-else />
      </q-page>
    </q-page-container>
  </q-layout>
</template>
