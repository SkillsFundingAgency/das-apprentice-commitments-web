
// Step by step

var $stepByStep = $('#step-by-step-navigation')
if ($stepByStep) {
    var stepByStepNavigation = new GOVUK.Modules.AppStepNav()
    stepByStepNavigation.start($stepByStep)
}
