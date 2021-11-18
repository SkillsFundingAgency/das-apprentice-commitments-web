@outerApi
Feature: ConfirmRolesAndResponsibilities
	As an apprentice i want to view my Roles and Responsibilities
	So I can confirm them

Scenario: The apprentice is authenticated and has not verified Roles and Responsibilities
	Given the apprentice has logged in
	And the apprentice has not verified their Roles and Responsibilities
	When accessing the RolesAndResponsibilities page
	Then the response status code should be Redirect
	And the redirect address is to the apprentice roles and responsibilities page

Scenario: The apprentice is authenticated and has verified Roles and Responsibilities
	Given the apprentice has logged in
	And the apprentice has verified their Roles and Responsibilities
	When accessing the RolesAndResponsibilities page
	Then the response status code should be Ok
	And the backlink will return to overview page 

Scenario: The apprentice is authenticated and has part verified Roles and Responsibilities
	Given the apprentice has logged in
	And the apprentice has confirmed the section ApprenticeRolesAndResponsibilitiesConfirmed
	When accessing the RolesAndResponsibilities page
	Then the response status code should be Redirect
	And the redirect address is to the apprentice roles and responsibilities page

Scenario: The apprentice is authenticated and has apprentices Roles and Responsibilities verified
	Given the apprentice has logged in
	And the apprentice has confirmed the section ApprenticeRolesAndResponsibilitiesConfirmed
	When accessing the RolesAndResponsibilities page
	Then the response status code should be Redirect
	And the redirect address is to the apprentice roles and responsibilities page

Scenario: The apprentice is authenticated and is verifying each Roles and Responsibilities section
	Given the apprentice has logged in
	And the apprentice has confirmed the section <Confirmation>
	When accessing the confirm RolesAndResponsibilities\<Section> page
	Then the response status code should be Ok
	And the section confirmed checkbox should be already checked
	And backlink with return to <Backlink>

	Examples:
	| Confirmation                                | Section | Backlink                                      |
	| ApprenticeRolesAndResponsibilitiesConfirmed | 1       | /apprenticeships/?                            |
	| EmployerRolesAndResponsibilitiesConfirmed   | 2       | /apprenticeships/?/rolesandresponsibilities/1 |
	| ProviderRolesAndResponsibilitiesConfirmed   | 3       | /apprenticeships/?/rolesandresponsibilities/2 |

Scenario: The apprentice is authenticated and is positively submitting each Roles and Responsibilities section
	Given the apprentice has logged in
	And the apprentice is confirming the section with true
	When submitting the RolesAndResponsibilities\<Section> page
	Then the <Confirmation> should be saved
	Then apprentice is redirected to <NextPage>

	Examples:
	| Confirmation                                | Section | NextPage |
	| ApprenticeRolesAndResponsibilitiesConfirmed | 1       | 2        |
	| EmployerRolesAndResponsibilitiesConfirmed   | 2       | 3        |
	| ProviderRolesAndResponsibilitiesConfirmed   | 3       | Index    |

Scenario: The apprentice is authenticated and has already fully confirmed Roles and Responsibilities
	Given the apprentice has logged in
	And the apprentice has verified their Roles and Responsibilities
	When accessing the confirm RolesAndResponsibilities\<Section> page
	Then apprentice is redirected to Index

	Examples:
	| Section |
	| 1       |
	| 2       |
	| 3       |

