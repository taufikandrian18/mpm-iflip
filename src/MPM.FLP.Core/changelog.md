# FLPDb Change Log
Berisi log perubahan database FLPDb

## 2022-01-26
### Add
| Table | Column | Type | Length |
| ----- | ------ | ---- | ------ |
| ContentBankCategories | Orders | int | |
| ContentBankCategories | AttachmentUrl | string| 256 |

## 2022-02-04
### Add
| Table | Column | Type | Length |
| ----- | ------ | ---- | ------ |
| ContentBanks | Caption | string | 256 |
| ContentBanks | ReadingTime | int | |
| ContentBanks | H1 | boolean | |
| ContentBanks | H2 | boolean | |
| ContentBanks | H3 | boolean | |
| ContentBanks | IsPublished | boolean | |


### Change
- Add newid() as default value for:
1. ContentBankCategories
2. ContentBanks
3. ContentBankDetails
4. ContentBankAssignees
5. ContentBankAssigneeProofs
- Change data type ContentBankAssignees, column GUIDEmployee from int to long
- Change column name ContentBankAssignees from AccountNumber to KodeDealerMPM
- Change data type ContentBankAssigneeProofs, column GUIDEmployee from int to long

## Phase 2 - 2022-02-07
### Add
| Table | Guid | 
| ----- | ------ | 
| ContentBankPlatforms | F8D6B7A2-3E38-4330-9B94-1052C6091D22 |
| ContentBankPlatforms | 9913B909-1622-4794-AED8-422596221638 |
| ContentBankPlatforms | 039109EC-5994-4EFF-A72D-6627840A61F6 |

## Phase 2 - 2022-02-07
### Add
| Table | Column | Type | Length |
| ----- | ------ | ---- | ------ |
| ContentBankAssignees | KodeDealerMPM | string | 10 |
| ContentBankAssignees | Status | int | |

## Phase 2 - 2022-02-07
### Add
| Table | 
| ----- | 
| SplashScreen |
| SplashScreenDetails |
| ApplicationFeature |
| ApplicationFeatureMapping |

## Phase 2 - 2022-02-11
### Add
| Table | 
| ----- | 
| ClaimProgramCampaigns |
| ClaimProgramCampaignPoints |
| ClaimProgramCampaignPrizes |
| ClaimProgramContents |
| ClaimProgramContentAttachments |
| ClaimProgramAssignees |
| ClaimProgramContentClaimers |

## Phase 2 - 2022-02-09
### Add
| Table |
| ----- |
| LogActivities |
| LogActivityDetails |

## Phase 2 - 2022-02-11
### Add
| Table | Column | Type | Length |
| ----- | ------ | ---- | ------ |
| ExternalUsers | Kota | string | 256 |
| ExternalUsers | CategoryH3 | string | 256 |
| ExternalUsers | KodeDealerH3 | string | 256 |
| DealerH3 |
| Sekolahs |
| TBSMUserGurus | 
| TBSMUserSiswas |

## Phase 2 - 2022-03-17
### Add
| Table |
| ----- |
| MechanicAssistants |
| MechanicAssistantCategories |
| MechanicAssistantAssignees |
| MechanicAssistantContacts |
| InboxMessageCategories |

## Phase 2 - 2022-03-17
### Add
| Table |
| ----- |
| BASTs |
| BASTAssignee |
| BASTAttachment |
| BASTCategories |
| BASTDetails |
| BASTReport |
| BASTReportAttachment |

### Add
| Table | Column | Type | Length |
| ----- | ------ | ---- | ------ |
| InboxMessages | GuidCategory | Guid | |
| InboxMessages | Link | string | 256 |
| ServiceTalkFlyers | IsH1 | bit | |
| ServiceTalkFlyers | IsH2 | bit | |
| ServiceTalkFlyers | IsH3 | bit | |
| ServiceTalkFlyers | IsTBSM | bit | |
