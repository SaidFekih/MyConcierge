<script setup>
import { ref } from 'vue'
import UnitesPage from './pages/UnitesPage.vue'
import ContratsPage from './pages/ContratsPage.vue'
import SoldesPage from './pages/SoldesPage.vue'
import UtilisateursPage from './pages/UtilisateursPage.vue'
import ReferenceTypesPage from './pages/ReferenceTypesPage.vue'

const onglet = ref('unites')
</script>

<template>
  <q-layout view="hHh lpR fFf" class="bg-grey-2">
    <q-header bordered class="bg-white text-grey-9">
      <q-toolbar>
        <q-icon name="apartment" color="primary" size="sm" class="q-mr-sm" />
        <q-toolbar-title class="text-weight-medium">MyConcierge</q-toolbar-title>
      </q-toolbar>
      <q-tabs v-model="onglet" align="left" active-color="primary" indicator-color="primary">
        <q-tab name="unites" label="Unités" />
        <q-tab name="utilisateurs" label="Utilisateurs" />
        <q-tab name="contrats" label="Contrats" />
        <q-tab name="soldes" label="Paiements et soldes" />
        <q-tab name="types" label="Types" />
      </q-tabs>
    </q-header>

    <q-page-container>
      <q-page padding>
        <!-- v-if : la page se recharge à chaque changement d'onglet -->
        <UnitesPage v-if="onglet === 'unites'" />
        <UtilisateursPage v-else-if="onglet === 'utilisateurs'" />
        <ContratsPage v-else-if="onglet === 'contrats'" />
        <SoldesPage v-else-if="onglet === 'soldes'" />
        <ReferenceTypesPage v-else />
      </q-page>
    </q-page-container>
  </q-layout>
</template>
