<template>
  <div id="app" class="uk-container uk-height-viewport">
    <LoginComponent />
    <CharacterSelectionComponent />
    <EquipmentComponent />
    <CharacterCreatorComponent />
    <AdministratorPanelComponent />
    <CharacterCustomizationComponent />
  </div>
</template>

<script>
import LoginComponent from "./components/LoginComponent.vue";
import CharacterSelectionComponent from "./components/CharacterSelectionComponent.vue";
import EquipmentComponent from "./components/EquipmentComponent.vue";
import CharacterCreatorComponent from "./components/CharacterCreatorComponent.vue";
import AdministratorPanelComponent from "./components/AdministratorPanelComponent.vue";
import CharacterCustomizationComponent from "./components/CharacterCustomizationComponent.vue";

export default {
  name: "App",
  components: {
    LoginComponent,
    CharacterSelectionComponent,
    EquipmentComponent,
    CharacterCreatorComponent,
    AdministratorPanelComponent,
    CharacterCustomizationComponent
  },
  data() {
    return {
      debug: false
    };
  },
};
</script>
