// Step by step

var $stepByStep = $('#step-by-step-navigation')
if ($stepByStep.length > 0) {
    var stepByStepNavigation = new GOVUK.Modules.AppStepNav()
    stepByStepNavigation.start($stepByStep)
}
