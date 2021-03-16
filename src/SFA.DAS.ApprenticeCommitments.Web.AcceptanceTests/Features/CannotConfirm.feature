@outerApi
Feature: CannotConfirm
	As an apprentice I want to see when I cannot confirm my apprenticeship
	So I know what to do

Scenario: The apprentice is authenticated and is on the CannotConfirm page
	Given the apprentice has logged in
	When accessing the CannotConfirm page
	Then the response status code should be Ok
	And the backlink is pointing to the confirm page