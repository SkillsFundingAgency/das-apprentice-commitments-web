@outerApi
Feature: ViewRolesAndResponsibilities
	As an apprentice who wants to view the roles and responsibities page from the My apprenticeship page
	So I can review what I previously agreed to

Scenario: The apprentice is authenticated and should see the Roles and responsibilies page
	Given the apprentice has logged in
	And the apprentice has verified they have read the page
	When accessing the Roles and responsibilities page from the my apprenticeship page
	Then the response status code should be OK
	And the back link is pointing to the my apprenticeship page
