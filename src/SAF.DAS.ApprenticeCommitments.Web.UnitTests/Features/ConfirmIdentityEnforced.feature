@outerApi
Feature: ConfirmIdentityEnforced
	Force a user to confirm their identity if they try to access an apprentices
	portal page but have not confirmed their identity

Scenario: Redirect to Confirm Your Identity from index
	Given a logged in user
	When the user has not already confirmed their identity
	And the user attempts to land on Apprenticeships index page
	Then redirect the user to the Confirm ID page

Scenario Outline: Redirect to Confirm Your Identity from subpages
	Given a logged in user
	When the user has not already confirmed their identity
	And the user attempts to land on personalised page <page>
	Then redirect the user to the Confirm ID page

	Examples: 
	| page            |
	| ConfirmYourEmployer |