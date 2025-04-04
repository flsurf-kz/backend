{
  "openapi": "3.0.1",
  "info": {
    "title": "SpakOfMind Flsurf",
    "version": "v1"
  },
  "paths": {
    "/api/auth/login": {
      "post": {
        "tags": [
          "Auth"
        ],
        "operationId": "login",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/LoginUserSchema"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/LoginUserSchema"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/LoginUserSchema"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success"
          }
        }
      }
    },
    "/api/auth/logout": {
      "post": {
        "tags": [
          "Auth"
        ],
        "operationId": "logout",
        "responses": {
          "200": {
            "description": "Success"
          }
        }
      }
    },
    "/api/auth/register": {
      "post": {
        "tags": [
          "Auth"
        ],
        "operationId": "register",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/RegisterUserSchema"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/RegisterUserSchema"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/RegisterUserSchema"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success"
          }
        }
      }
    },
    "/api/auth/external-login/{provider}": {
      "get": {
        "tags": [
          "Auth"
        ],
        "operationId": "external-login",
        "parameters": [
          {
            "name": "provider",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success"
          }
        }
      }
    },
    "/api/auth/external-login-callback": {
      "get": {
        "tags": [
          "Auth"
        ],
        "operationId": "external-login-callback",
        "responses": {
          "200": {
            "description": "Success"
          }
        }
      }
    },
    "/api/category/create": {
      "post": {
        "tags": [
          "Category"
        ],
        "operationId": "create",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateCategoryCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateCategoryCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateCategoryCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/category/update": {
      "post": {
        "tags": [
          "Category"
        ],
        "operationId": "update",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateCategoryCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateCategoryCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateCategoryCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/category/{categoryId}": {
      "delete": {
        "tags": [
          "Category"
        ],
        "operationId": "category",
        "parameters": [
          {
            "name": "categoryId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/category/list": {
      "get": {
        "tags": [
          "Category"
        ],
        "operationId": "list",
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/CategoryEntity"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/CategoryEntity"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/CategoryEntity"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/client-profile/order-info/{userId}": {
      "get": {
        "tags": [
          "ClientProfile"
        ],
        "operationId": "order-info",
        "parameters": [
          {
            "name": "userId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ClientJobInfo"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ClientJobInfo"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ClientJobInfo"
                }
              }
            }
          }
        }
      }
    },
    "/api/client-profile/create": {
      "post": {
        "tags": [
          "ClientProfile"
        ],
        "operationId": "create2",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateClientProfileCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateClientProfileCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateClientProfileCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/client-profile/suspend": {
      "post": {
        "tags": [
          "ClientProfile"
        ],
        "operationId": "suspend",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/SuspendClientProfileCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/SuspendClientProfileCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/SuspendClientProfileCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/client-profile/update": {
      "post": {
        "tags": [
          "ClientProfile"
        ],
        "operationId": "update2",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateClientProfileCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateClientProfileCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateClientProfileCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contest/create": {
      "post": {
        "tags": [
          "Contest"
        ],
        "operationId": "create3",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateContestCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateContestCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateContestCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contest/approve": {
      "post": {
        "tags": [
          "Contest"
        ],
        "operationId": "approve",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/ApproveContestCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/ApproveContestCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/ApproveContestCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contest/start": {
      "post": {
        "tags": [
          "Contest"
        ],
        "operationId": "start",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/StartContestCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/StartContestCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/StartContestCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contest/end": {
      "post": {
        "tags": [
          "Contest"
        ],
        "operationId": "end",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/EndContestCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/EndContestCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/EndContestCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contest/delete": {
      "delete": {
        "tags": [
          "Contest"
        ],
        "operationId": "delete",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteContestCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteContestCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteContestCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contest/select-winner": {
      "post": {
        "tags": [
          "Contest"
        ],
        "operationId": "select-winner",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/SelectContestWinnerCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/SelectContestWinnerCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/SelectContestWinnerCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contest/submit-entry": {
      "post": {
        "tags": [
          "Contest"
        ],
        "operationId": "submit-entry",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/SubmitContestEntryCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/SubmitContestEntryCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/SubmitContestEntryCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contest/delete-entry": {
      "delete": {
        "tags": [
          "Contest"
        ],
        "operationId": "delete-entry",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteContestEntryCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteContestEntryCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteContestEntryCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contest/update": {
      "post": {
        "tags": [
          "Contest"
        ],
        "operationId": "update3",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateContestCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateContestCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateContestCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contest/{id}": {
      "get": {
        "tags": [
          "Contest"
        ],
        "operationId": "contest",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ContestEntity"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ContestEntity"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ContestEntity"
                }
              }
            }
          }
        }
      }
    },
    "/api/contest/list": {
      "get": {
        "tags": [
          "Contest"
        ],
        "operationId": "list2",
        "parameters": [
          {
            "name": "start",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 0
            }
          },
          {
            "name": "end",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 10
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/ContestEntity"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/ContestEntity"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/ContestEntity"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/contract/create": {
      "post": {
        "tags": [
          "Contract"
        ],
        "operationId": "create4",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateContractCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateContractCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateContractCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contract/client-accept-finish": {
      "post": {
        "tags": [
          "Contract"
        ],
        "operationId": "client-accept-finish",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/ClientAcceptFinishContractCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/ClientAcceptFinishContractCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/ClientAcceptFinishContractCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contract/client-close": {
      "post": {
        "tags": [
          "Contract"
        ],
        "operationId": "client-close",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/ClientCloseContractCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/ClientCloseContractCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/ClientCloseContractCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contract/client-reject-completion": {
      "post": {
        "tags": [
          "Contract"
        ],
        "operationId": "client-reject-completion",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/ClientRejectContractCompletionCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/ClientRejectContractCompletionCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/ClientRejectContractCompletionCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contract/freelancer-accept": {
      "post": {
        "tags": [
          "Contract"
        ],
        "operationId": "freelancer-accept",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/FreelancerAcceptContractCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/FreelancerAcceptContractCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/FreelancerAcceptContractCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contract/freelancer-finish": {
      "post": {
        "tags": [
          "Contract"
        ],
        "operationId": "freelancer-finish",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/FreelancerFinishContractCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/FreelancerFinishContractCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/FreelancerFinishContractCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contract/accept-dispute": {
      "post": {
        "tags": [
          "Contract"
        ],
        "operationId": "accept-dispute",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/AcceptDisputeCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/AcceptDisputeCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/AcceptDisputeCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contract/initiate-dispute": {
      "post": {
        "tags": [
          "Contract"
        ],
        "operationId": "initiate-dispute",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/InitiateDisputeCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/InitiateDisputeCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/InitiateDisputeCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contract/resolve-dispute": {
      "post": {
        "tags": [
          "Contract"
        ],
        "operationId": "resolve-dispute",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/ResolveDisputeCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/ResolveDisputeCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/ResolveDisputeCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contract/force-cancel": {
      "post": {
        "tags": [
          "Contract"
        ],
        "operationId": "force-cancel",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/ForceContractCancelCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/ForceContractCancelCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/ForceContractCancelCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/contract/{id}": {
      "get": {
        "tags": [
          "Contract"
        ],
        "operationId": "contract",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ContractEntity"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ContractEntity"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ContractEntity"
                }
              }
            }
          }
        }
      }
    },
    "/api/contract/list": {
      "get": {
        "tags": [
          "Contract"
        ],
        "operationId": "list3",
        "parameters": [
          {
            "name": "start",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 0
            }
          },
          {
            "name": "end",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 10
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/ContractEntity"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/ContractEntity"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/ContractEntity"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/files/upload": {
      "post": {
        "tags": [
          "FileControllers"
        ],
        "operationId": "upload",
        "requestBody": {
          "content": {
            "multipart/form-data": {
              "schema": {
                "type": "object",
                "properties": {
                  "file": {
                    "type": "string",
                    "format": "binary"
                  }
                }
              },
              "encoding": {
                "file": {
                  "style": "form"
                }
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/FileEntity"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/FileEntity"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/FileEntity"
                }
              }
            }
          }
        }
      }
    },
    "/api/files/download/{fileId}": {
      "get": {
        "tags": [
          "FileControllers"
        ],
        "operationId": "download",
        "parameters": [
          {
            "name": "fileId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success"
          }
        }
      }
    },
    "/api/freelancer-profile/create": {
      "post": {
        "tags": [
          "FreelancerProfile"
        ],
        "operationId": "create5",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateFreelancerProfileCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateFreelancerProfileCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateFreelancerProfileCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/freelancer-profile/update": {
      "post": {
        "tags": [
          "FreelancerProfile"
        ],
        "operationId": "update4",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateFreelancerProfileCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateFreelancerProfileCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateFreelancerProfileCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/freelancer-profile/hide": {
      "post": {
        "tags": [
          "FreelancerProfile"
        ],
        "operationId": "hide",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/HideFreelancerProfileCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/HideFreelancerProfileCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/HideFreelancerProfileCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/freelancer-profile/{userId}": {
      "get": {
        "tags": [
          "FreelancerProfile"
        ],
        "operationId": "freelancer-profile",
        "parameters": [
          {
            "name": "userId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/FreelancerProfileEntity"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/FreelancerProfileEntity"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/FreelancerProfileEntity"
                }
              }
            }
          }
        }
      }
    },
    "/api/freelancer-profile/list": {
      "get": {
        "tags": [
          "FreelancerProfile"
        ],
        "operationId": "list4",
        "parameters": [
          {
            "name": "start",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 0
            }
          },
          {
            "name": "end",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 10
            }
          },
          {
            "name": "skills",
            "in": "query",
            "schema": {
              "type": "array",
              "items": {
                "type": "string"
              }
            }
          },
          {
            "name": "minCost",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          },
          {
            "name": "maxCost",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          },
          {
            "name": "minReviews",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          },
          {
            "name": "maxReviews",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/FreelancerProfileEntity"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/FreelancerProfileEntity"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/FreelancerProfileEntity"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/freelancer-team/create": {
      "post": {
        "tags": [
          "FreelancerTeam"
        ],
        "operationId": "create6",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateFreelancerTeamCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateFreelancerTeamCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateFreelancerTeamCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/freelancer-team/update": {
      "post": {
        "tags": [
          "FreelancerTeam"
        ],
        "operationId": "update5",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateFreelancerTeamCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateFreelancerTeamCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateFreelancerTeamCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/freelancer-team/delete": {
      "delete": {
        "tags": [
          "FreelancerTeam"
        ],
        "operationId": "delete2",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteFreelancerTeamCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteFreelancerTeamCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteFreelancerTeamCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/freelancer-team/list": {
      "get": {
        "tags": [
          "FreelancerTeam"
        ],
        "operationId": "list5",
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/FreelancerTeamEntity"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/FreelancerTeamEntity"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/FreelancerTeamEntity"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/job/create": {
      "post": {
        "tags": [
          "Job"
        ],
        "operationId": "create7",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateJobCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateJobCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateJobCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/job/update": {
      "post": {
        "tags": [
          "Job"
        ],
        "operationId": "update6",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateJobCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateJobCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateJobCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/job/delete": {
      "delete": {
        "tags": [
          "Job"
        ],
        "operationId": "delete3",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteJobCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteJobCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteJobCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/job/{id}": {
      "get": {
        "tags": [
          "Job"
        ],
        "operationId": "job",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/Job"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/Job"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/Job"
                }
              }
            }
          }
        }
      }
    },
    "/api/job/list": {
      "get": {
        "tags": [
          "Job"
        ],
        "operationId": "list6",
        "parameters": [
          {
            "name": "start",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 0
            }
          },
          {
            "name": "end",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 10
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/Job"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/Job"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/Job"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/job/bookmark": {
      "post": {
        "tags": [
          "Job"
        ],
        "operationId": "bookmark",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/BookmarkJobCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/BookmarkJobCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/BookmarkJobCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/job/hide": {
      "post": {
        "tags": [
          "Job"
        ],
        "operationId": "hide2",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/HideJobCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/HideJobCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/HideJobCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/job/submit-proposal": {
      "post": {
        "tags": [
          "Job"
        ],
        "operationId": "submit-proposal",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/SubmitProposalCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/SubmitProposalCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/SubmitProposalCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/job/update-proposal": {
      "post": {
        "tags": [
          "Job"
        ],
        "operationId": "update-proposal",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateProposalCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateProposalCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateProposalCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/job/withdraw-proposal": {
      "post": {
        "tags": [
          "Job"
        ],
        "operationId": "withdraw-proposal",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/WithdrawProposalCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/WithdrawProposalCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/WithdrawProposalCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/job/bookmarks": {
      "get": {
        "tags": [
          "Job"
        ],
        "operationId": "bookmarks",
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/JobEntity"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/JobEntity"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/JobEntity"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/notification/user/{userId}": {
      "get": {
        "tags": [
          "Notification"
        ],
        "operationId": "userAll",
        "parameters": [
          {
            "name": "userId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          },
          {
            "name": "Start",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          },
          {
            "name": "Ends",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/NotificationEntity"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/NotificationEntity"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/NotificationEntity"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/notification": {
      "post": {
        "tags": [
          "Notification"
        ],
        "operationId": "notification",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateNotificationCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateNotificationCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateNotificationCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/portfolio-project/create": {
      "post": {
        "tags": [
          "PortfolioProject"
        ],
        "operationId": "create8",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/AddPortfolioProjectCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/AddPortfolioProjectCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/AddPortfolioProjectCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/portfolio-project/update": {
      "post": {
        "tags": [
          "PortfolioProject"
        ],
        "operationId": "update7",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdatePortfolioProjectCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdatePortfolioProjectCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdatePortfolioProjectCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/portfolio-project/delete": {
      "delete": {
        "tags": [
          "PortfolioProject"
        ],
        "operationId": "delete4",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/DeletePortfolioProjectCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/DeletePortfolioProjectCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/DeletePortfolioProjectCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/portfolio-project/list": {
      "get": {
        "tags": [
          "PortfolioProject"
        ],
        "operationId": "list7",
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/PortfolioProjectEntity"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/PortfolioProjectEntity"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/PortfolioProjectEntity"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/skill/create": {
      "post": {
        "tags": [
          "Skill"
        ],
        "operationId": "create9",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateSkillsCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateSkillsCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateSkillsCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/skill/update": {
      "post": {
        "tags": [
          "Skill"
        ],
        "operationId": "update8",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateSkillsCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateSkillsCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateSkillsCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/skill/delete": {
      "delete": {
        "tags": [
          "Skill"
        ],
        "operationId": "delete5",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteSkillsCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteSkillsCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteSkillsCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/skill/list": {
      "get": {
        "tags": [
          "Skill"
        ],
        "operationId": "list8",
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/SkillEntity"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/SkillEntity"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/SkillEntity"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/stuff/user/{userId}/block": {
      "post": {
        "tags": [
          "StaffControllers"
        ],
        "operationId": "block",
        "parameters": [
          {
            "name": "userId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "boolean"
                }
              },
              "application/json": {
                "schema": {
                  "type": "boolean"
                }
              },
              "text/json": {
                "schema": {
                  "type": "boolean"
                }
              }
            }
          }
        }
      }
    },
    "/api/stuff/user/{userId}/warn": {
      "post": {
        "tags": [
          "StaffControllers"
        ],
        "operationId": "warn",
        "parameters": [
          {
            "name": "userId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/WarnUserScheme"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/WarnUserScheme"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/WarnUserScheme"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "boolean"
                }
              },
              "application/json": {
                "schema": {
                  "type": "boolean"
                }
              },
              "text/json": {
                "schema": {
                  "type": "boolean"
                }
              }
            }
          }
        }
      }
    },
    "/api/stuff/ticket": {
      "post": {
        "tags": [
          "StaffControllers"
        ],
        "operationId": "ticketPOST",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateTicketDto"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateTicketDto"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateTicketDto"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/TicketEntity"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/TicketEntity"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/TicketEntity"
                }
              }
            }
          }
        }
      },
      "get": {
        "tags": [
          "StaffControllers"
        ],
        "operationId": "ticketAll",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/GetTicketsDto"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/GetTicketsDto"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/GetTicketsDto"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/TicketEntity"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/TicketEntity"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/TicketEntity"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/stuff/ticket/{ticketId}": {
      "get": {
        "tags": [
          "StaffControllers"
        ],
        "operationId": "ticketGET",
        "parameters": [
          {
            "name": "ticketId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/TicketEntity"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/TicketEntity"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/TicketEntity"
                }
              }
            }
          }
        }
      }
    },
    "/api/task/create": {
      "post": {
        "tags": [
          "Task"
        ],
        "operationId": "create10",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateTaskCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateTaskCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateTaskCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/task/complete": {
      "post": {
        "tags": [
          "Task"
        ],
        "operationId": "complete",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CompleteTaskCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CompleteTaskCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CompleteTaskCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/task/react": {
      "post": {
        "tags": [
          "Task"
        ],
        "operationId": "react",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/ReactToTaskCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/ReactToTaskCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/ReactToTaskCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/task/update": {
      "post": {
        "tags": [
          "Task"
        ],
        "operationId": "update9",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateTaskCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateTaskCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateTaskCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/task/delete": {
      "delete": {
        "tags": [
          "Task"
        ],
        "operationId": "delete6",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteTaskCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteTaskCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteTaskCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/task/list/{contractId}": {
      "get": {
        "tags": [
          "Task"
        ],
        "operationId": "list9",
        "parameters": [
          {
            "name": "contractId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          },
          {
            "name": "start",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 0
            }
          },
          {
            "name": "end",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 10
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/TransactionEntity"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/TransactionEntity"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/TransactionEntity"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/transaction/handle": {
      "post": {
        "tags": [
          "Transaction"
        ],
        "operationId": "handle",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/HandleTransactionCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/HandleTransactionCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/HandleTransactionCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/transaction/list": {
      "get": {
        "tags": [
          "Transaction"
        ],
        "operationId": "list10",
        "parameters": [
          {
            "name": "start",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 0
            }
          },
          {
            "name": "end",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 10
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/TransactionEntity"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/TransactionEntity"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/TransactionEntity"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/transaction/providers": {
      "get": {
        "tags": [
          "Transaction"
        ],
        "operationId": "providers",
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/TransactionProviderEntity"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/TransactionProviderEntity"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/TransactionProviderEntity"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/transaction/deposit-result": {
      "post": {
        "tags": [
          "Transaction"
        ],
        "operationId": "deposit-result",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/GatewayResultCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/GatewayResultCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/GatewayResultCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/transaction/refund": {
      "post": {
        "tags": [
          "Transaction"
        ],
        "operationId": "refund",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/RefundTransactionCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/RefundTransactionCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/RefundTransactionCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/transaction/withdrawal-result": {
      "post": {
        "tags": [
          "Transaction"
        ],
        "operationId": "withdrawal-result",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/GatewayResultCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/GatewayResultCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/GatewayResultCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/transaction/start": {
      "post": {
        "tags": [
          "Transaction"
        ],
        "operationId": "start2",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/StartPaymentFlowCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/StartPaymentFlowCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/StartPaymentFlowCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/user/{userId}": {
      "patch": {
        "tags": [
          "UserControllers"
        ],
        "operationId": "userPATCH",
        "parameters": [
          {
            "name": "userId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateUserCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateUserCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateUserCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "boolean"
                }
              },
              "application/json": {
                "schema": {
                  "type": "boolean"
                }
              },
              "text/json": {
                "schema": {
                  "type": "boolean"
                }
              }
            }
          }
        }
      },
      "get": {
        "tags": [
          "UserControllers"
        ],
        "operationId": "userGET",
        "parameters": [
          {
            "name": "userId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/UserEntity"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/UserEntity"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/UserEntity"
                }
              }
            }
          }
        }
      }
    },
    "/api/user/me": {
      "patch": {
        "tags": [
          "UserControllers"
        ],
        "operationId": "mePATCH",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateUserCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateUserCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateUserCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "boolean"
                }
              },
              "application/json": {
                "schema": {
                  "type": "boolean"
                }
              },
              "text/json": {
                "schema": {
                  "type": "boolean"
                }
              }
            }
          }
        }
      },
      "get": {
        "tags": [
          "UserControllers"
        ],
        "operationId": "meGET",
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/UserEntity"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/UserEntity"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/UserEntity"
                }
              }
            }
          }
        }
      }
    },
    "/api/wallet/balance-operation": {
      "post": {
        "tags": [
          "Wallet"
        ],
        "operationId": "balance-operation",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/BalanceOperationCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/BalanceOperationCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/BalanceOperationCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/wallet/{walletId}": {
      "get": {
        "tags": [
          "Wallet"
        ],
        "operationId": "wallet",
        "parameters": [
          {
            "name": "walletId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/WalletEntity"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/WalletEntity"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/WalletEntity"
                }
              }
            }
          }
        }
      }
    },
    "/api/wallet/block": {
      "post": {
        "tags": [
          "Wallet"
        ],
        "operationId": "block2",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/BlockWalletCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/BlockWalletCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/BlockWalletCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/work-session/start": {
      "post": {
        "tags": [
          "WorkSession"
        ],
        "operationId": "start3",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/StartWorkSessionCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/StartWorkSessionCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/StartWorkSessionCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/work-session/submit": {
      "post": {
        "tags": [
          "WorkSession"
        ],
        "operationId": "submit",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/SubmitWorkSessionCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/SubmitWorkSessionCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/SubmitWorkSessionCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/work-session/end": {
      "post": {
        "tags": [
          "WorkSession"
        ],
        "operationId": "end2",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/EndWorkSessionCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/EndWorkSessionCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/EndWorkSessionCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/work-session/approve": {
      "post": {
        "tags": [
          "WorkSession"
        ],
        "operationId": "approve2",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/ApproveWorkSessionCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/ApproveWorkSessionCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/ApproveWorkSessionCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/work-session/react": {
      "post": {
        "tags": [
          "WorkSession"
        ],
        "operationId": "react2",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/ReactToWorkSessionCommand"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/ReactToWorkSessionCommand"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/ReactToWorkSessionCommand"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommandResult"
                }
              }
            }
          }
        }
      }
    },
    "/api/work-session/{id}": {
      "get": {
        "tags": [
          "WorkSession"
        ],
        "operationId": "work-session",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/WorkSessionEntity"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/WorkSessionEntity"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/WorkSessionEntity"
                }
              }
            }
          }
        }
      }
    },
    "/api/work-session/list": {
      "get": {
        "tags": [
          "WorkSession"
        ],
        "operationId": "list11",
        "parameters": [
          {
            "name": "contractId",
            "in": "query",
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          },
          {
            "name": "start",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 0
            }
          },
          {
            "name": "end",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 10
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/WorkSessionEntity"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/WorkSessionEntity"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/WorkSessionEntity"
                  }
                }
              }
            }
          }
        }
      }
    }
  },
  "components": {
    "schemas": {
      "AcceptDisputeCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "disputeId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "AddPortfolioProjectCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "name": {
            "type": "string",
            "nullable": true
          },
          "userRole": {
            "type": "string",
            "nullable": true
          },
          "skills": {
            "type": "array",
            "nullable": true,
            "items": {
              "type": "string",
              "format": "uuid"
            }
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "files": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CreateFileDto"
            }
          }
        }
      },
      "ApproveContestCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "contestId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "ApproveWorkSessionCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "sessionId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "Assembly": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "definedTypes": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/TypeInfo"
            }
          },
          "exportedTypes": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/Type"
            }
          },
          "codeBase": {
            "type": "string",
            "readOnly": true,
            "deprecated": true,
            "nullable": true
          },
          "entryPoint": {
            "$ref": "#/components/schemas/MethodInfo"
          },
          "fullName": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "imageRuntimeVersion": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "isDynamic": {
            "type": "boolean",
            "readOnly": true
          },
          "location": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "reflectionOnly": {
            "type": "boolean",
            "readOnly": true
          },
          "isCollectible": {
            "type": "boolean",
            "readOnly": true
          },
          "isFullyTrusted": {
            "type": "boolean",
            "readOnly": true
          },
          "customAttributes": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CustomAttributeData"
            }
          },
          "escapedCodeBase": {
            "type": "string",
            "readOnly": true,
            "deprecated": true,
            "nullable": true
          },
          "manifestModule": {
            "$ref": "#/components/schemas/Module"
          },
          "modules": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/Module"
            }
          },
          "globalAssemblyCache": {
            "type": "boolean",
            "readOnly": true,
            "deprecated": true
          },
          "hostContext": {
            "type": "integer",
            "readOnly": true,
            "format": "int64"
          },
          "securityRuleSet": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              0,
              1,
              2
            ]
          }
        }
      },
      "BalanceOperationCommand": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "balance",
          "walletId"
        ],
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "walletId": {
            "type": "string",
            "readOnly": true,
            "format": "uuid"
          },
          "balance": {
            "$ref": "#/components/schemas/Money"
          },
          "balanceOperationType": {
            "type": "string",
            "readOnly": true,
            "enum": [
              "Freeze",
              "Unfreeze",
              "PendingIncome",
              "Deposit",
              "Withdrawl"
            ]
          }
        }
      },
      "BlockWalletCommand": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "reason",
          "walletId"
        ],
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "walletId": {
            "type": "string",
            "format": "uuid"
          },
          "reason": {
            "type": "string",
            "enum": [
              "None",
              "FraudSuspicion",
              "LegalIssue",
              "UserRequest"
            ]
          }
        }
      },
      "BookmarkJobCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "jobId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "CategoryEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "id"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "name": {
            "type": "string",
            "nullable": true
          },
          "slug": {
            "type": "string",
            "nullable": true
          },
          "parentCategoryId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "parentCategory": {
            "$ref": "#/components/schemas/CategoryEntity"
          },
          "subCategories": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CategoryEntity"
            }
          }
        }
      },
      "ClientAcceptFinishContractCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "contractId": {
            "type": "string",
            "readOnly": true,
            "format": "uuid"
          }
        }
      },
      "ClientCloseContractCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "contractId": {
            "type": "string",
            "format": "uuid"
          },
          "reason": {
            "type": "string",
            "nullable": true
          }
        }
      },
      "ClientJobInfo": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "userId": {
            "type": "string",
            "format": "uuid"
          },
          "name": {
            "type": "string",
            "nullable": true
          },
          "avatar": {
            "$ref": "#/components/schemas/FileEntity"
          },
          "isVerified": {
            "type": "boolean"
          },
          "activeJobs": {
            "type": "integer",
            "format": "int32"
          },
          "closedJobs": {
            "type": "integer",
            "format": "int32"
          },
          "arbitrationJobs": {
            "type": "integer",
            "format": "int32"
          },
          "activeContracts": {
            "type": "integer",
            "format": "int32"
          },
          "completedContracts": {
            "type": "integer",
            "format": "int32"
          },
          "arbitrationContracts": {
            "type": "integer",
            "format": "int32"
          },
          "registeredAt": {
            "type": "string",
            "nullable": true
          },
          "lastActiveAt": {
            "type": "string",
            "nullable": true
          },
          "isPhoneVerified": {
            "type": "boolean"
          },
          "hasPremium": {
            "type": "boolean"
          }
        }
      },
      "ClientRejectContractCompletionCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "contractId": {
            "type": "string",
            "format": "uuid"
          },
          "reason": {
            "type": "string",
            "nullable": true
          }
        }
      },
      "CommandResult": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "message": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "id": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "ids": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "type": "string"
            }
          },
          "status": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              100,
              101,
              102,
              103,
              200,
              201,
              202,
              203,
              204,
              205,
              206,
              207,
              208,
              226,
              300,
              301,
              302,
              303,
              304,
              305,
              306,
              307,
              308,
              400,
              401,
              402,
              403,
              404,
              405,
              406,
              407,
              408,
              409,
              410,
              411,
              412,
              413,
              414,
              415,
              416,
              417,
              421,
              422,
              423,
              424,
              426,
              428,
              429,
              431,
              451,
              500,
              501,
              502,
              503,
              504,
              505,
              506,
              507,
              508,
              510,
              511
            ]
          },
          "isSuccess": {
            "type": "boolean",
            "readOnly": true
          }
        }
      },
      "CompleteTaskCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "taskId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "ConstructorInfo": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "name": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "declaringType": {
            "$ref": "#/components/schemas/Type"
          },
          "reflectedType": {
            "$ref": "#/components/schemas/Type"
          },
          "module": {
            "$ref": "#/components/schemas/Module"
          },
          "customAttributes": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CustomAttributeData"
            }
          },
          "isCollectible": {
            "type": "boolean",
            "readOnly": true
          },
          "metadataToken": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          },
          "attributes": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              0,
              1,
              2,
              3,
              4,
              5,
              6,
              7,
              8,
              16,
              32,
              64,
              128,
              256,
              512,
              1024,
              2048,
              4096,
              8192,
              16384,
              32768,
              53248
            ]
          },
          "methodImplementationFlags": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              0,
              1,
              2,
              3,
              4,
              8,
              16,
              32,
              64,
              128,
              256,
              512,
              4096,
              65535
            ]
          },
          "callingConvention": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              1,
              2,
              3,
              32,
              64
            ]
          },
          "isAbstract": {
            "type": "boolean",
            "readOnly": true
          },
          "isConstructor": {
            "type": "boolean",
            "readOnly": true
          },
          "isFinal": {
            "type": "boolean",
            "readOnly": true
          },
          "isHideBySig": {
            "type": "boolean",
            "readOnly": true
          },
          "isSpecialName": {
            "type": "boolean",
            "readOnly": true
          },
          "isStatic": {
            "type": "boolean",
            "readOnly": true
          },
          "isVirtual": {
            "type": "boolean",
            "readOnly": true
          },
          "isAssembly": {
            "type": "boolean",
            "readOnly": true
          },
          "isFamily": {
            "type": "boolean",
            "readOnly": true
          },
          "isFamilyAndAssembly": {
            "type": "boolean",
            "readOnly": true
          },
          "isFamilyOrAssembly": {
            "type": "boolean",
            "readOnly": true
          },
          "isPrivate": {
            "type": "boolean",
            "readOnly": true
          },
          "isPublic": {
            "type": "boolean",
            "readOnly": true
          },
          "isConstructedGenericMethod": {
            "type": "boolean",
            "readOnly": true
          },
          "isGenericMethod": {
            "type": "boolean",
            "readOnly": true
          },
          "isGenericMethodDefinition": {
            "type": "boolean",
            "readOnly": true
          },
          "containsGenericParameters": {
            "type": "boolean",
            "readOnly": true
          },
          "methodHandle": {
            "$ref": "#/components/schemas/RuntimeMethodHandle"
          },
          "isSecurityCritical": {
            "type": "boolean",
            "readOnly": true
          },
          "isSecuritySafeCritical": {
            "type": "boolean",
            "readOnly": true
          },
          "isSecurityTransparent": {
            "type": "boolean",
            "readOnly": true
          },
          "memberType": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              1,
              2,
              4,
              8,
              16,
              32,
              64,
              128,
              191
            ]
          }
        }
      },
      "ContestEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "id"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "employerId": {
            "type": "string",
            "format": "uuid"
          },
          "employer": {
            "$ref": "#/components/schemas/UserEntity"
          },
          "title": {
            "type": "string",
            "nullable": true
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "prizePool": {
            "$ref": "#/components/schemas/Money"
          },
          "startDate": {
            "type": "string",
            "format": "date-time"
          },
          "endDate": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "status": {
            "type": "string",
            "enum": [
              "Draft",
              "Moderation",
              "Approved",
              "Open",
              "Ended",
              "WinnerSelected"
            ]
          },
          "winnerEntryId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "isResultPublic": {
            "type": "boolean"
          },
          "isEntriesPublic": {
            "type": "boolean"
          },
          "files": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/FileEntity"
            }
          }
        }
      },
      "ContractEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "id"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "freelancerId": {
            "type": "string",
            "format": "uuid"
          },
          "freelancer": {
            "$ref": "#/components/schemas/UserEntity"
          },
          "employerId": {
            "type": "string",
            "format": "uuid"
          },
          "employer": {
            "$ref": "#/components/schemas/UserEntity"
          },
          "startDate": {
            "type": "string",
            "format": "date-time"
          },
          "endDate": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "budget": {
            "$ref": "#/components/schemas/Money"
          },
          "status": {
            "type": "string",
            "enum": [
              "PendingApproval",
              "Active",
              "Paused",
              "Completed",
              "Disputed",
              "Cancelled",
              "Expired",
              "Closed",
              "PendingFinishApproval"
            ]
          },
          "costPerHour": {
            "$ref": "#/components/schemas/Money"
          },
          "budgetType": {
            "type": "string",
            "enum": [
              "Hourly",
              "Fixed"
            ]
          },
          "tasks": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/TaskEntity"
            }
          },
          "workSessions": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/WorkSessionEntity"
            }
          },
          "totalWorkSessions": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          },
          "totalHoursWorked": {
            "type": "number",
            "readOnly": true,
            "format": "double"
          },
          "remainingBudget": {
            "$ref": "#/components/schemas/Money"
          },
          "paymentSchedule": {
            "type": "string",
            "enum": [
              "Milestone",
              "Weekly",
              "Monthly",
              "OnCompletion"
            ]
          },
          "isPaused": {
            "type": "boolean"
          },
          "pauseReason": {
            "type": "string",
            "nullable": true
          },
          "contractTerms": {
            "type": "string",
            "nullable": true
          },
          "bonus": {
            "type": "number",
            "format": "double",
            "nullable": true
          },
          "disputeId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          }
        }
      },
      "CreateCategoryCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "name": {
            "type": "string",
            "nullable": true
          },
          "slug": {
            "type": "string",
            "nullable": true
          },
          "tags": {
            "type": "string",
            "nullable": true
          },
          "parentCategoryId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          }
        }
      },
      "CreateClientProfileCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "companyName": {
            "type": "string",
            "nullable": true
          },
          "companyDescription": {
            "type": "string",
            "nullable": true
          },
          "companyWebsite": {
            "type": "string",
            "nullable": true
          },
          "location": {
            "type": "string",
            "nullable": true
          },
          "companyLogo": {
            "$ref": "#/components/schemas/CreateFileDto"
          },
          "employerType": {
            "type": "string",
            "enum": [
              "Company",
              "Indivdual"
            ]
          },
          "phoneNumber": {
            "type": "string",
            "format": "tel",
            "nullable": true
          }
        }
      },
      "CreateContestCommand": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "description",
          "prizePool",
          "title"
        ],
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "title": {
            "type": "string",
            "minLength": 1
          },
          "description": {
            "type": "string",
            "minLength": 1
          },
          "prizePool": {
            "type": "number",
            "format": "double"
          },
          "isResultPublic": {
            "type": "boolean"
          }
        }
      },
      "CreateContractCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "freelancerId": {
            "type": "string",
            "format": "uuid"
          },
          "jobId": {
            "type": "string",
            "format": "uuid"
          },
          "budget": {
            "type": "number",
            "format": "double",
            "nullable": true
          },
          "costPerHour": {
            "type": "number",
            "format": "double",
            "nullable": true
          },
          "budgetType": {
            "type": "string",
            "enum": [
              "Hourly",
              "Fixed"
            ]
          },
          "paymentSchedule": {
            "type": "string",
            "enum": [
              "Milestone",
              "Weekly",
              "Monthly",
              "OnCompletion"
            ]
          },
          "contractTerms": {
            "type": "string",
            "nullable": true
          },
          "endDate": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          }
        }
      },
      "CreateFileDto": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "downloadUrl": {
            "type": "string",
            "nullable": true
          },
          "fileId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "name": {
            "type": "string",
            "nullable": true
          },
          "mimeType": {
            "type": "string",
            "nullable": true
          },
          "stream": {
            "$ref": "#/components/schemas/Stream"
          }
        }
      },
      "CreateFreelancerProfileCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "experience": {
            "type": "string",
            "nullable": true
          },
          "hourlyRate": {
            "type": "number",
            "format": "double"
          },
          "resume": {
            "type": "string",
            "nullable": true
          }
        }
      },
      "CreateFreelancerTeamCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "name": {
            "type": "string",
            "nullable": true
          },
          "description": {
            "type": "string",
            "nullable": true
          }
        }
      },
      "CreateJobCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "title": {
            "type": "string",
            "nullable": true
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "requiredSkillIds": {
            "type": "array",
            "nullable": true,
            "items": {
              "type": "string",
              "format": "uuid"
            }
          },
          "categoryId": {
            "type": "string",
            "format": "uuid"
          },
          "budget": {
            "type": "number",
            "format": "double",
            "nullable": true
          },
          "hourlyRate": {
            "type": "number",
            "format": "double",
            "nullable": true
          },
          "duration": {
            "type": "integer",
            "format": "int32",
            "nullable": true
          },
          "budgetType": {
            "type": "string",
            "enum": [
              "Hourly",
              "Fixed"
            ]
          },
          "level": {
            "type": "string",
            "enum": [
              "Beginner",
              "Intermediate",
              "Expert"
            ]
          },
          "expirationDate": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "files": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CreateFileDto"
            }
          }
        }
      },
      "CreateNotificationCommand": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "data",
          "text",
          "title"
        ],
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "title": {
            "type": "string",
            "minLength": 1
          },
          "text": {
            "type": "string",
            "minLength": 1
          },
          "data": {
            "type": "object",
            "additionalProperties": {
              "type": "string"
            }
          },
          "userId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "role": {
            "type": "string",
            "nullable": true,
            "enum": [
              "User",
              "Moderator",
              "Admin",
              "Superadmin"
            ]
          },
          "type": {
            "type": "string",
            "nullable": true,
            "enum": [
              "Freelancer",
              "Client",
              "NonUser"
            ]
          }
        }
      },
      "CreateSkillsCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "skillNames": {
            "type": "array",
            "nullable": true,
            "items": {
              "type": "string"
            }
          }
        }
      },
      "CreateTaskCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "contractId": {
            "type": "string",
            "format": "uuid"
          },
          "title": {
            "type": "string",
            "nullable": true
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "priority": {
            "type": "integer",
            "format": "int32"
          }
        }
      },
      "CreateTicketDto": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "subject",
          "text"
        ],
        "properties": {
          "subject": {
            "type": "string",
            "minLength": 1
          },
          "text": {
            "type": "string",
            "minLength": 1
          },
          "files": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CreateFileDto"
            }
          },
          "priorityScore": {
            "type": "number",
            "format": "double"
          },
          "linkedDisputeId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "title": {
            "type": "string",
            "nullable": true
          }
        }
      },
      "CustomAttributeData": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "attributeType": {
            "$ref": "#/components/schemas/Type"
          },
          "constructor": {
            "$ref": "#/components/schemas/ConstructorInfo"
          },
          "constructorArguments": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CustomAttributeTypedArgument"
            }
          },
          "namedArguments": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CustomAttributeNamedArgument"
            }
          }
        }
      },
      "CustomAttributeNamedArgument": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "memberInfo": {
            "$ref": "#/components/schemas/MemberInfo"
          },
          "typedValue": {
            "$ref": "#/components/schemas/CustomAttributeTypedArgument"
          },
          "memberName": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "isField": {
            "type": "boolean",
            "readOnly": true
          }
        }
      },
      "CustomAttributeTypedArgument": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "argumentType": {
            "$ref": "#/components/schemas/Type"
          },
          "value": {
            "nullable": true
          }
        }
      },
      "DeleteContestCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "contestId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "DeleteContestEntryCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "contestEntryId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "DeleteFreelancerTeamCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "teamId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "DeleteJobCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "jobId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "DeletePortfolioProjectCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "projectId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "DeleteSkillsCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "skillIds": {
            "type": "array",
            "nullable": true,
            "items": {
              "type": "string",
              "format": "uuid"
            }
          },
          "skillNames": {
            "type": "array",
            "nullable": true,
            "items": {
              "type": "string"
            }
          }
        }
      },
      "DeleteTaskCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "taskId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "EndContestCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "contestId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "EndWorkSessionCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "sessionId": {
            "type": "string",
            "format": "uuid"
          },
          "selectedFiles": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CreateFileDto"
            }
          }
        }
      },
      "EventInfo": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "name": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "declaringType": {
            "$ref": "#/components/schemas/Type"
          },
          "reflectedType": {
            "$ref": "#/components/schemas/Type"
          },
          "module": {
            "$ref": "#/components/schemas/Module"
          },
          "customAttributes": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CustomAttributeData"
            }
          },
          "isCollectible": {
            "type": "boolean",
            "readOnly": true
          },
          "metadataToken": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          },
          "memberType": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              1,
              2,
              4,
              8,
              16,
              32,
              64,
              128,
              191
            ]
          },
          "attributes": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              0,
              512,
              1024
            ]
          },
          "isSpecialName": {
            "type": "boolean",
            "readOnly": true
          },
          "addMethod": {
            "$ref": "#/components/schemas/MethodInfo"
          },
          "removeMethod": {
            "$ref": "#/components/schemas/MethodInfo"
          },
          "raiseMethod": {
            "$ref": "#/components/schemas/MethodInfo"
          },
          "isMulticast": {
            "type": "boolean",
            "readOnly": true
          },
          "eventHandlerType": {
            "$ref": "#/components/schemas/Type"
          }
        }
      },
      "FeeContext": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "isContractCancellation": {
            "type": "boolean",
            "readOnly": true
          },
          "isAdminOverride": {
            "type": "boolean",
            "readOnly": true
          }
        }
      },
      "FieldInfo": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "name": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "declaringType": {
            "$ref": "#/components/schemas/Type"
          },
          "reflectedType": {
            "$ref": "#/components/schemas/Type"
          },
          "module": {
            "$ref": "#/components/schemas/Module"
          },
          "customAttributes": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CustomAttributeData"
            }
          },
          "isCollectible": {
            "type": "boolean",
            "readOnly": true
          },
          "metadataToken": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          },
          "memberType": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              1,
              2,
              4,
              8,
              16,
              32,
              64,
              128,
              191
            ]
          },
          "attributes": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              0,
              1,
              2,
              3,
              4,
              5,
              6,
              7,
              16,
              32,
              64,
              128,
              256,
              512,
              1024,
              4096,
              8192,
              32768,
              38144
            ]
          },
          "fieldType": {
            "$ref": "#/components/schemas/Type"
          },
          "isInitOnly": {
            "type": "boolean",
            "readOnly": true
          },
          "isLiteral": {
            "type": "boolean",
            "readOnly": true
          },
          "isNotSerialized": {
            "type": "boolean",
            "readOnly": true,
            "deprecated": true
          },
          "isPinvokeImpl": {
            "type": "boolean",
            "readOnly": true
          },
          "isSpecialName": {
            "type": "boolean",
            "readOnly": true
          },
          "isStatic": {
            "type": "boolean",
            "readOnly": true
          },
          "isAssembly": {
            "type": "boolean",
            "readOnly": true
          },
          "isFamily": {
            "type": "boolean",
            "readOnly": true
          },
          "isFamilyAndAssembly": {
            "type": "boolean",
            "readOnly": true
          },
          "isFamilyOrAssembly": {
            "type": "boolean",
            "readOnly": true
          },
          "isPrivate": {
            "type": "boolean",
            "readOnly": true
          },
          "isPublic": {
            "type": "boolean",
            "readOnly": true
          },
          "isSecurityCritical": {
            "type": "boolean",
            "readOnly": true
          },
          "isSecuritySafeCritical": {
            "type": "boolean",
            "readOnly": true
          },
          "isSecurityTransparent": {
            "type": "boolean",
            "readOnly": true
          },
          "fieldHandle": {
            "$ref": "#/components/schemas/RuntimeFieldHandle"
          }
        }
      },
      "FileEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "fileName",
          "filePath",
          "id",
          "mimeType"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "fileName": {
            "type": "string",
            "maxLength": 256,
            "minLength": 1
          },
          "filePath": {
            "type": "string",
            "minLength": 1
          },
          "mimeType": {
            "type": "string",
            "maxLength": 128,
            "minLength": 1
          },
          "size": {
            "type": "integer",
            "format": "int64"
          }
        }
      },
      "ForceContractCancelCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "contractId": {
            "type": "string",
            "format": "uuid"
          },
          "reason": {
            "type": "string",
            "nullable": true
          }
        }
      },
      "FreelancerAcceptContractCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "contractId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "FreelancerFinishContractCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "contractId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "FreelancerProfileEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "id"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "userId": {
            "type": "string",
            "format": "uuid"
          },
          "user": {
            "$ref": "#/components/schemas/UserEntity"
          },
          "skills": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/SkillEntity"
            }
          },
          "experience": {
            "type": "string",
            "nullable": true
          },
          "portfolioProjects": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/PortfolioProjectEntity"
            }
          },
          "resume": {
            "type": "string",
            "nullable": true
          },
          "costPerHour": {
            "type": "number",
            "format": "double"
          },
          "availability": {
            "type": "string",
            "enum": [
              "Open",
              "Busy",
              "Vacation"
            ]
          },
          "rating": {
            "type": "number",
            "format": "float"
          },
          "isHidden": {
            "type": "boolean"
          },
          "reviews": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/JobReviewEntity"
            }
          }
        }
      },
      "FreelancerTeamEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "id"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "name": {
            "type": "string",
            "nullable": true
          },
          "participants": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/UserEntity"
            }
          },
          "closed": {
            "type": "boolean",
            "readOnly": true
          },
          "closedReason": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "avatarId": {
            "type": "string",
            "format": "uuid"
          },
          "avatar": {
            "$ref": "#/components/schemas/FileEntity"
          },
          "owner": {
            "$ref": "#/components/schemas/UserEntity"
          },
          "ownerId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "GatewayResultCommand": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "amount",
          "currency",
          "fee",
          "gatewayTransactionId",
          "internalTransactionId",
          "success"
        ],
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "success": {
            "type": "boolean"
          },
          "gatewayTransactionId": {
            "type": "string",
            "minLength": 1
          },
          "internalTransactionId": {
            "type": "string",
            "minLength": 1
          },
          "customId": {
            "type": "string",
            "nullable": true
          },
          "amount": {
            "type": "number",
            "format": "double"
          },
          "fee": {
            "type": "number",
            "format": "double"
          },
          "currency": {
            "type": "string",
            "enum": [
              "RussianRuble",
              "Dollar",
              "Euro"
            ]
          },
          "status": {
            "type": "string",
            "nullable": true
          },
          "failureReason": {
            "type": "string",
            "nullable": true
          },
          "metadata": {
            "type": "object",
            "nullable": true,
            "additionalProperties": {
              "type": "string"
            }
          }
        }
      },
      "GetTicketsDto": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "start": {
            "type": "integer",
            "format": "int32"
          },
          "ends": {
            "type": "integer",
            "format": "int32"
          },
          "userId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "subject": {
            "type": "string",
            "nullable": true
          },
          "isAssignedToMe": {
            "type": "boolean",
            "nullable": true
          }
        }
      },
      "HandleTransactionCommand": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "transactionId"
        ],
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "transactionId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "HideFreelancerProfileCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "userId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "HideJobCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "jobId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "ICustomAttributeProvider": {
        "type": "object",
        "additionalProperties": false
      },
      "InitiateDisputeCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "contractId": {
            "type": "string",
            "format": "uuid"
          },
          "reason": {
            "type": "string",
            "nullable": true
          }
        }
      },
      "IntPtr": {
        "type": "object",
        "additionalProperties": false
      },
      "Job": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "type": {
            "$ref": "#/components/schemas/Type"
          },
          "method": {
            "$ref": "#/components/schemas/MethodInfo"
          },
          "args": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {}
          },
          "queue": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "arguments": {
            "type": "array",
            "readOnly": true,
            "deprecated": true,
            "nullable": true,
            "items": {
              "type": "string"
            }
          }
        }
      },
      "JobEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "id"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "employerId": {
            "type": "string",
            "format": "uuid"
          },
          "employer": {
            "$ref": "#/components/schemas/UserEntity"
          },
          "title": {
            "type": "string",
            "nullable": true
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "requiredSkills": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/SkillEntity"
            }
          },
          "category": {
            "$ref": "#/components/schemas/CategoryEntity"
          },
          "categoryId": {
            "type": "string",
            "format": "uuid"
          },
          "payout": {
            "$ref": "#/components/schemas/Money"
          },
          "expirationDate": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "duration": {
            "type": "integer",
            "format": "int32",
            "nullable": true
          },
          "status": {
            "type": "string",
            "enum": [
              "Open",
              "Expired",
              "Closed",
              "Accepted",
              "InContract",
              "Draft",
              "Completed",
              "WaitingFreelancerApproval"
            ]
          },
          "proposals": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/ProposalEntity"
            }
          },
          "level": {
            "type": "string",
            "enum": [
              "Beginner",
              "Intermediate",
              "Expert"
            ]
          },
          "budgetType": {
            "type": "string",
            "enum": [
              "Hourly",
              "Fixed"
            ]
          },
          "publicationDate": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "paymentVerified": {
            "type": "boolean"
          },
          "files": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/FileEntity"
            }
          },
          "isHidden": {
            "type": "boolean"
          }
        }
      },
      "JobReviewEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "comment",
          "id",
          "job",
          "jobId",
          "rating",
          "reviewDate",
          "reviewer",
          "reviewerId",
          "target",
          "targetId"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "reviewerId": {
            "type": "string",
            "format": "uuid"
          },
          "reviewer": {
            "$ref": "#/components/schemas/UserEntity"
          },
          "targetId": {
            "type": "string",
            "format": "uuid"
          },
          "target": {
            "$ref": "#/components/schemas/UserEntity"
          },
          "jobId": {
            "type": "string",
            "format": "uuid"
          },
          "job": {
            "$ref": "#/components/schemas/JobEntity"
          },
          "rating": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          },
          "comment": {
            "type": "string",
            "minLength": 1
          },
          "reviewDate": {
            "type": "string",
            "format": "date-time"
          }
        }
      },
      "LoginUserSchema": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "email",
          "password"
        ],
        "properties": {
          "email": {
            "type": "string",
            "minLength": 1
          },
          "password": {
            "type": "string",
            "minLength": 1
          },
          "rememberMe": {
            "type": "boolean"
          }
        }
      },
      "MemberInfo": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "memberType": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              1,
              2,
              4,
              8,
              16,
              32,
              64,
              128,
              191
            ]
          },
          "name": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "declaringType": {
            "$ref": "#/components/schemas/Type"
          },
          "reflectedType": {
            "$ref": "#/components/schemas/Type"
          },
          "module": {
            "$ref": "#/components/schemas/Module"
          },
          "customAttributes": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CustomAttributeData"
            }
          },
          "isCollectible": {
            "type": "boolean",
            "readOnly": true
          },
          "metadataToken": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          }
        }
      },
      "MethodBase": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "memberType": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              1,
              2,
              4,
              8,
              16,
              32,
              64,
              128,
              191
            ]
          },
          "name": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "declaringType": {
            "$ref": "#/components/schemas/Type"
          },
          "reflectedType": {
            "$ref": "#/components/schemas/Type"
          },
          "module": {
            "$ref": "#/components/schemas/Module"
          },
          "customAttributes": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CustomAttributeData"
            }
          },
          "isCollectible": {
            "type": "boolean",
            "readOnly": true
          },
          "metadataToken": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          },
          "attributes": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              0,
              1,
              2,
              3,
              4,
              5,
              6,
              7,
              8,
              16,
              32,
              64,
              128,
              256,
              512,
              1024,
              2048,
              4096,
              8192,
              16384,
              32768,
              53248
            ]
          },
          "methodImplementationFlags": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              0,
              1,
              2,
              3,
              4,
              8,
              16,
              32,
              64,
              128,
              256,
              512,
              4096,
              65535
            ]
          },
          "callingConvention": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              1,
              2,
              3,
              32,
              64
            ]
          },
          "isAbstract": {
            "type": "boolean",
            "readOnly": true
          },
          "isConstructor": {
            "type": "boolean",
            "readOnly": true
          },
          "isFinal": {
            "type": "boolean",
            "readOnly": true
          },
          "isHideBySig": {
            "type": "boolean",
            "readOnly": true
          },
          "isSpecialName": {
            "type": "boolean",
            "readOnly": true
          },
          "isStatic": {
            "type": "boolean",
            "readOnly": true
          },
          "isVirtual": {
            "type": "boolean",
            "readOnly": true
          },
          "isAssembly": {
            "type": "boolean",
            "readOnly": true
          },
          "isFamily": {
            "type": "boolean",
            "readOnly": true
          },
          "isFamilyAndAssembly": {
            "type": "boolean",
            "readOnly": true
          },
          "isFamilyOrAssembly": {
            "type": "boolean",
            "readOnly": true
          },
          "isPrivate": {
            "type": "boolean",
            "readOnly": true
          },
          "isPublic": {
            "type": "boolean",
            "readOnly": true
          },
          "isConstructedGenericMethod": {
            "type": "boolean",
            "readOnly": true
          },
          "isGenericMethod": {
            "type": "boolean",
            "readOnly": true
          },
          "isGenericMethodDefinition": {
            "type": "boolean",
            "readOnly": true
          },
          "containsGenericParameters": {
            "type": "boolean",
            "readOnly": true
          },
          "methodHandle": {
            "$ref": "#/components/schemas/RuntimeMethodHandle"
          },
          "isSecurityCritical": {
            "type": "boolean",
            "readOnly": true
          },
          "isSecuritySafeCritical": {
            "type": "boolean",
            "readOnly": true
          },
          "isSecurityTransparent": {
            "type": "boolean",
            "readOnly": true
          }
        }
      },
      "MethodInfo": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "name": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "declaringType": {
            "$ref": "#/components/schemas/Type"
          },
          "reflectedType": {
            "$ref": "#/components/schemas/Type"
          },
          "module": {
            "$ref": "#/components/schemas/Module"
          },
          "customAttributes": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CustomAttributeData"
            }
          },
          "isCollectible": {
            "type": "boolean",
            "readOnly": true
          },
          "metadataToken": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          },
          "attributes": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              0,
              1,
              2,
              3,
              4,
              5,
              6,
              7,
              8,
              16,
              32,
              64,
              128,
              256,
              512,
              1024,
              2048,
              4096,
              8192,
              16384,
              32768,
              53248
            ]
          },
          "methodImplementationFlags": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              0,
              1,
              2,
              3,
              4,
              8,
              16,
              32,
              64,
              128,
              256,
              512,
              4096,
              65535
            ]
          },
          "callingConvention": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              1,
              2,
              3,
              32,
              64
            ]
          },
          "isAbstract": {
            "type": "boolean",
            "readOnly": true
          },
          "isConstructor": {
            "type": "boolean",
            "readOnly": true
          },
          "isFinal": {
            "type": "boolean",
            "readOnly": true
          },
          "isHideBySig": {
            "type": "boolean",
            "readOnly": true
          },
          "isSpecialName": {
            "type": "boolean",
            "readOnly": true
          },
          "isStatic": {
            "type": "boolean",
            "readOnly": true
          },
          "isVirtual": {
            "type": "boolean",
            "readOnly": true
          },
          "isAssembly": {
            "type": "boolean",
            "readOnly": true
          },
          "isFamily": {
            "type": "boolean",
            "readOnly": true
          },
          "isFamilyAndAssembly": {
            "type": "boolean",
            "readOnly": true
          },
          "isFamilyOrAssembly": {
            "type": "boolean",
            "readOnly": true
          },
          "isPrivate": {
            "type": "boolean",
            "readOnly": true
          },
          "isPublic": {
            "type": "boolean",
            "readOnly": true
          },
          "isConstructedGenericMethod": {
            "type": "boolean",
            "readOnly": true
          },
          "isGenericMethod": {
            "type": "boolean",
            "readOnly": true
          },
          "isGenericMethodDefinition": {
            "type": "boolean",
            "readOnly": true
          },
          "containsGenericParameters": {
            "type": "boolean",
            "readOnly": true
          },
          "methodHandle": {
            "$ref": "#/components/schemas/RuntimeMethodHandle"
          },
          "isSecurityCritical": {
            "type": "boolean",
            "readOnly": true
          },
          "isSecuritySafeCritical": {
            "type": "boolean",
            "readOnly": true
          },
          "isSecurityTransparent": {
            "type": "boolean",
            "readOnly": true
          },
          "memberType": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              1,
              2,
              4,
              8,
              16,
              32,
              64,
              128,
              191
            ]
          },
          "returnParameter": {
            "$ref": "#/components/schemas/ParameterInfo"
          },
          "returnType": {
            "$ref": "#/components/schemas/Type"
          },
          "returnTypeCustomAttributes": {
            "$ref": "#/components/schemas/ICustomAttributeProvider"
          }
        }
      },
      "Module": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "assembly": {
            "$ref": "#/components/schemas/Assembly"
          },
          "fullyQualifiedName": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "name": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "mdStreamVersion": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          },
          "moduleVersionId": {
            "type": "string",
            "readOnly": true,
            "format": "uuid"
          },
          "scopeName": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "moduleHandle": {
            "$ref": "#/components/schemas/ModuleHandle"
          },
          "customAttributes": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CustomAttributeData"
            }
          },
          "metadataToken": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          }
        }
      },
      "ModuleHandle": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "mdStreamVersion": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          }
        }
      },
      "Money": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "amount",
          "currency"
        ],
        "properties": {
          "amount": {
            "type": "number",
            "format": "double"
          },
          "currency": {
            "type": "string",
            "enum": [
              "RussianRuble",
              "Dollar",
              "Euro"
            ]
          }
        }
      },
      "NotificationEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "id",
          "text",
          "title",
          "toUserId"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "title": {
            "type": "string",
            "minLength": 1
          },
          "text": {
            "type": "string",
            "minLength": 1
          },
          "fromUserId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "toUserId": {
            "type": "string",
            "format": "uuid"
          },
          "type": {
            "type": "string",
            "enum": [
              "System",
              "Payment",
              "Other"
            ]
          },
          "data": {
            "type": "string",
            "nullable": true
          },
          "icon": {
            "$ref": "#/components/schemas/FileEntity"
          }
        }
      },
      "ParameterInfo": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "attributes": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              0,
              1,
              2,
              4,
              8,
              16,
              4096,
              8192,
              16384,
              32768,
              61440
            ]
          },
          "member": {
            "$ref": "#/components/schemas/MemberInfo"
          },
          "name": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "parameterType": {
            "$ref": "#/components/schemas/Type"
          },
          "position": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          },
          "isIn": {
            "type": "boolean",
            "readOnly": true
          },
          "isLcid": {
            "type": "boolean",
            "readOnly": true
          },
          "isOptional": {
            "type": "boolean",
            "readOnly": true
          },
          "isOut": {
            "type": "boolean",
            "readOnly": true
          },
          "isRetval": {
            "type": "boolean",
            "readOnly": true
          },
          "defaultValue": {
            "readOnly": true,
            "nullable": true
          },
          "rawDefaultValue": {
            "readOnly": true,
            "nullable": true
          },
          "hasDefaultValue": {
            "type": "boolean",
            "readOnly": true
          },
          "customAttributes": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CustomAttributeData"
            }
          },
          "metadataToken": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          }
        }
      },
      "PaymentSystemEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "id",
          "name"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "name": {
            "type": "string",
            "minLength": 1
          },
          "isActive": {
            "type": "boolean"
          }
        }
      },
      "PortfolioProjectEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "id"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "name": {
            "type": "string",
            "nullable": true
          },
          "userRole": {
            "type": "string",
            "nullable": true
          },
          "skills": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/SkillEntity"
            }
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "images": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/FileEntity"
            }
          },
          "hidden": {
            "type": "boolean"
          },
          "userId": {
            "type": "string",
            "format": "uuid"
          },
          "user": {
            "$ref": "#/components/schemas/UserEntity"
          }
        }
      },
      "PropertyInfo": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "name": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "declaringType": {
            "$ref": "#/components/schemas/Type"
          },
          "reflectedType": {
            "$ref": "#/components/schemas/Type"
          },
          "module": {
            "$ref": "#/components/schemas/Module"
          },
          "customAttributes": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CustomAttributeData"
            }
          },
          "isCollectible": {
            "type": "boolean",
            "readOnly": true
          },
          "metadataToken": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          },
          "memberType": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              1,
              2,
              4,
              8,
              16,
              32,
              64,
              128,
              191
            ]
          },
          "propertyType": {
            "$ref": "#/components/schemas/Type"
          },
          "attributes": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              0,
              512,
              1024,
              4096,
              8192,
              16384,
              32768,
              62464
            ]
          },
          "isSpecialName": {
            "type": "boolean",
            "readOnly": true
          },
          "canRead": {
            "type": "boolean",
            "readOnly": true
          },
          "canWrite": {
            "type": "boolean",
            "readOnly": true
          },
          "getMethod": {
            "$ref": "#/components/schemas/MethodInfo"
          },
          "setMethod": {
            "$ref": "#/components/schemas/MethodInfo"
          }
        }
      },
      "ProposalEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "id"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "jobId": {
            "type": "string",
            "format": "uuid"
          },
          "job": {
            "$ref": "#/components/schemas/JobEntity"
          },
          "freelancerId": {
            "type": "string",
            "format": "uuid"
          },
          "freelancer": {
            "$ref": "#/components/schemas/UserEntity"
          },
          "proposedRate": {
            "type": "number",
            "format": "double"
          },
          "coverLetter": {
            "type": "string",
            "nullable": true
          },
          "status": {
            "type": "string",
            "enum": [
              "Pending",
              "Accepted",
              "Hidden"
            ]
          },
          "files": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/FileEntity"
            }
          }
        }
      },
      "ReactToTaskCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "taskId": {
            "type": "string",
            "format": "uuid"
          },
          "approve": {
            "type": "boolean"
          },
          "newTitle": {
            "type": "string",
            "nullable": true
          },
          "newDescription": {
            "type": "string",
            "nullable": true
          }
        }
      },
      "ReactToWorkSessionCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "workSessionId": {
            "type": "string",
            "format": "uuid"
          },
          "isApproved": {
            "type": "boolean"
          },
          "clientComment": {
            "type": "string",
            "nullable": true
          }
        }
      },
      "RefundTransactionCommand": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "transactionId"
        ],
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "transactionId": {
            "type": "string",
            "format": "uuid"
          },
          "isContractCancellation": {
            "type": "boolean"
          }
        }
      },
      "RegisterUserSchema": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "email",
          "name",
          "password",
          "surname"
        ],
        "properties": {
          "name": {
            "type": "string",
            "maxLength": 52,
            "minLength": 0
          },
          "surname": {
            "type": "string",
            "maxLength": 52,
            "minLength": 0
          },
          "phone": {
            "type": "string",
            "format": "tel",
            "nullable": true
          },
          "password": {
            "type": "string",
            "minLength": 1
          },
          "email": {
            "type": "string",
            "format": "email",
            "minLength": 1
          }
        }
      },
      "ResolveDisputeCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "disputeId": {
            "type": "string",
            "format": "uuid"
          },
          "strategy": {
            "type": "integer",
            "format": "int32",
            "enum": [
              0,
              1,
              2
            ]
          },
          "moderatorComment": {
            "type": "string",
            "nullable": true
          },
          "blockFreelancerWallet": {
            "type": "boolean"
          },
          "blockClientWallet": {
            "type": "boolean"
          },
          "blockFreelancerOrders": {
            "type": "boolean"
          },
          "blockUntil": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          }
        }
      },
      "RuntimeFieldHandle": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "value": {
            "$ref": "#/components/schemas/IntPtr"
          }
        }
      },
      "RuntimeMethodHandle": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "value": {
            "$ref": "#/components/schemas/IntPtr"
          }
        }
      },
      "RuntimeTypeHandle": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "value": {
            "$ref": "#/components/schemas/IntPtr"
          }
        }
      },
      "SelectContestWinnerCommand": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "contestId",
          "entryId"
        ],
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "contestId": {
            "type": "string",
            "format": "uuid"
          },
          "entryId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "SkillEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "id"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "name": {
            "type": "string",
            "nullable": true
          }
        }
      },
      "StartContestCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "contestId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "StartPaymentFlowCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "amount": {
            "$ref": "#/components/schemas/Money"
          },
          "flow": {
            "type": "string",
            "enum": [
              "Incoming",
              "Outgoing",
              "Internal"
            ]
          },
          "type": {
            "type": "string",
            "enum": [
              "Deposit",
              "Withdrawal",
              "Transfer",
              "Refund",
              "SystemAdjustment",
              "Bonus",
              "Penalty"
            ]
          },
          "providerId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "StartWorkSessionCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "contractId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "Stream": {
        "type": "string",
        "format": "binary",
        "additionalProperties": false,
        "properties": {
          "canRead": {
            "type": "boolean",
            "readOnly": true
          },
          "canWrite": {
            "type": "boolean",
            "readOnly": true
          },
          "canSeek": {
            "type": "boolean",
            "readOnly": true
          },
          "canTimeout": {
            "type": "boolean",
            "readOnly": true
          },
          "length": {
            "type": "integer",
            "readOnly": true,
            "format": "int64"
          },
          "position": {
            "type": "integer",
            "format": "int64"
          },
          "readTimeout": {
            "type": "integer",
            "format": "int32"
          },
          "writeTimeout": {
            "type": "integer",
            "format": "int32"
          }
        }
      },
      "StructLayoutAttribute": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "typeId": {
            "readOnly": true,
            "nullable": true
          },
          "value": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              0,
              2,
              3
            ]
          }
        }
      },
      "SubmitContestEntryCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "contestId": {
            "type": "string",
            "format": "uuid"
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "submissionFiles": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CreateFileDto"
            }
          }
        }
      },
      "SubmitProposalCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "jobId": {
            "type": "string",
            "format": "uuid"
          },
          "coverLetter": {
            "type": "string",
            "nullable": true
          },
          "proposedRate": {
            "type": "number",
            "format": "double",
            "nullable": true
          }
        }
      },
      "SubmitWorkSessionCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "sessionId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "SuspendClientProfileCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "clientId": {
            "type": "string",
            "format": "uuid"
          },
          "reason": {
            "type": "string",
            "nullable": true
          }
        }
      },
      "TaskEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "id"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "contractId": {
            "type": "string",
            "format": "uuid"
          },
          "contract": {
            "$ref": "#/components/schemas/ContractEntity"
          },
          "taskTitle": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "taskDescription": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "status": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "priority": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          },
          "creationDate": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "completionDate": {
            "type": "string",
            "readOnly": true,
            "format": "date-time",
            "nullable": true
          },
          "isCompleted": {
            "type": "boolean",
            "readOnly": true
          },
          "isApproved": {
            "type": "boolean",
            "readOnly": true
          },
          "isInRevision": {
            "type": "boolean",
            "readOnly": true
          }
        }
      },
      "TicketCommentEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "createdBy",
          "files",
          "id",
          "isDeleted",
          "text",
          "ticketId"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "createdBy": {
            "$ref": "#/components/schemas/UserEntity"
          },
          "text": {
            "type": "string",
            "minLength": 1
          },
          "parentCommentId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "isDeleted": {
            "type": "boolean"
          },
          "ticketId": {
            "type": "string",
            "format": "uuid"
          },
          "files": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/FileEntity"
            }
          }
        }
      },
      "TicketEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "id",
          "status",
          "subject",
          "text"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "text": {
            "type": "string",
            "minLength": 1
          },
          "subject": {
            "type": "string",
            "minLength": 1
          },
          "files": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/FileEntity"
            }
          },
          "answeredCommentId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "status": {
            "type": "string",
            "readOnly": true,
            "enum": [
              "Open",
              "Closed",
              "InProgress"
            ]
          },
          "assignedUser": {
            "$ref": "#/components/schemas/UserEntity"
          },
          "assignedUserId": {
            "type": "string",
            "readOnly": true,
            "format": "uuid",
            "nullable": true
          },
          "comments": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/TicketCommentEntity"
            }
          },
          "closedById": {
            "type": "string",
            "readOnly": true,
            "format": "uuid",
            "nullable": true
          },
          "createdBy": {
            "$ref": "#/components/schemas/UserEntity"
          }
        }
      },
      "TransactionEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "id"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "walletId": {
            "type": "string",
            "readOnly": true,
            "format": "uuid"
          },
          "antoganistTransactionId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "rawAmount": {
            "$ref": "#/components/schemas/Money"
          },
          "netAmount": {
            "$ref": "#/components/schemas/Money"
          },
          "appliedFee": {
            "$ref": "#/components/schemas/Money"
          },
          "status": {
            "type": "string",
            "readOnly": true,
            "enum": [
              "Pending",
              "Processing",
              "Completed",
              "Failed",
              "Cancelled",
              "Expired",
              "Reversed"
            ]
          },
          "type": {
            "type": "string",
            "readOnly": true,
            "enum": [
              "Deposit",
              "Withdrawal",
              "Transfer",
              "Refund",
              "SystemAdjustment",
              "Bonus",
              "Penalty"
            ]
          },
          "flow": {
            "type": "string",
            "readOnly": true,
            "enum": [
              "Incoming",
              "Outgoing",
              "Internal"
            ]
          },
          "props": {
            "$ref": "#/components/schemas/TransactionPropsEntity"
          },
          "frozenUntil": {
            "type": "string",
            "readOnly": true,
            "format": "date-time",
            "nullable": true
          },
          "comment": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "completedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "provider": {
            "$ref": "#/components/schemas/TransactionProviderEntity"
          }
        }
      },
      "TransactionPropsEntity": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "paymentUrl": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "successUrl": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "paymentGateway": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "feeContext": {
            "$ref": "#/components/schemas/FeeContext"
          }
        }
      },
      "TransactionProviderEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "feePercent",
          "id",
          "logo",
          "name",
          "systems"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "name": {
            "type": "string",
            "minLength": 1
          },
          "feePercent": {
            "type": "number",
            "format": "double"
          },
          "systems": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/PaymentSystemEntity"
            }
          },
          "logo": {
            "$ref": "#/components/schemas/FileEntity"
          }
        }
      },
      "Type": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "name": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "customAttributes": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CustomAttributeData"
            }
          },
          "isCollectible": {
            "type": "boolean",
            "readOnly": true
          },
          "metadataToken": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          },
          "isInterface": {
            "type": "boolean",
            "readOnly": true
          },
          "memberType": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              1,
              2,
              4,
              8,
              16,
              32,
              64,
              128,
              191
            ]
          },
          "namespace": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "assemblyQualifiedName": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "fullName": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "assembly": {
            "$ref": "#/components/schemas/Assembly"
          },
          "module": {
            "$ref": "#/components/schemas/Module"
          },
          "isNested": {
            "type": "boolean",
            "readOnly": true
          },
          "declaringType": {
            "$ref": "#/components/schemas/Type"
          },
          "declaringMethod": {
            "$ref": "#/components/schemas/MethodBase"
          },
          "reflectedType": {
            "$ref": "#/components/schemas/Type"
          },
          "underlyingSystemType": {
            "$ref": "#/components/schemas/Type"
          },
          "isTypeDefinition": {
            "type": "boolean",
            "readOnly": true
          },
          "isArray": {
            "type": "boolean",
            "readOnly": true
          },
          "isByRef": {
            "type": "boolean",
            "readOnly": true
          },
          "isPointer": {
            "type": "boolean",
            "readOnly": true
          },
          "isConstructedGenericType": {
            "type": "boolean",
            "readOnly": true
          },
          "isGenericParameter": {
            "type": "boolean",
            "readOnly": true
          },
          "isGenericTypeParameter": {
            "type": "boolean",
            "readOnly": true
          },
          "isGenericMethodParameter": {
            "type": "boolean",
            "readOnly": true
          },
          "isGenericType": {
            "type": "boolean",
            "readOnly": true
          },
          "isGenericTypeDefinition": {
            "type": "boolean",
            "readOnly": true
          },
          "isSZArray": {
            "type": "boolean",
            "readOnly": true
          },
          "isVariableBoundArray": {
            "type": "boolean",
            "readOnly": true
          },
          "isByRefLike": {
            "type": "boolean",
            "readOnly": true
          },
          "isFunctionPointer": {
            "type": "boolean",
            "readOnly": true
          },
          "isUnmanagedFunctionPointer": {
            "type": "boolean",
            "readOnly": true
          },
          "hasElementType": {
            "type": "boolean",
            "readOnly": true
          },
          "genericTypeArguments": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/Type"
            }
          },
          "genericParameterPosition": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          },
          "genericParameterAttributes": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              0,
              1,
              2,
              3,
              4,
              8,
              16,
              28
            ]
          },
          "attributes": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              0,
              1,
              2,
              3,
              4,
              5,
              6,
              7,
              8,
              16,
              24,
              32,
              128,
              256,
              1024,
              2048,
              4096,
              8192,
              16384,
              65536,
              131072,
              196608,
              262144,
              264192,
              1048576,
              12582912
            ]
          },
          "isAbstract": {
            "type": "boolean",
            "readOnly": true
          },
          "isImport": {
            "type": "boolean",
            "readOnly": true
          },
          "isSealed": {
            "type": "boolean",
            "readOnly": true
          },
          "isSpecialName": {
            "type": "boolean",
            "readOnly": true
          },
          "isClass": {
            "type": "boolean",
            "readOnly": true
          },
          "isNestedAssembly": {
            "type": "boolean",
            "readOnly": true
          },
          "isNestedFamANDAssem": {
            "type": "boolean",
            "readOnly": true
          },
          "isNestedFamily": {
            "type": "boolean",
            "readOnly": true
          },
          "isNestedFamORAssem": {
            "type": "boolean",
            "readOnly": true
          },
          "isNestedPrivate": {
            "type": "boolean",
            "readOnly": true
          },
          "isNestedPublic": {
            "type": "boolean",
            "readOnly": true
          },
          "isNotPublic": {
            "type": "boolean",
            "readOnly": true
          },
          "isPublic": {
            "type": "boolean",
            "readOnly": true
          },
          "isAutoLayout": {
            "type": "boolean",
            "readOnly": true
          },
          "isExplicitLayout": {
            "type": "boolean",
            "readOnly": true
          },
          "isLayoutSequential": {
            "type": "boolean",
            "readOnly": true
          },
          "isAnsiClass": {
            "type": "boolean",
            "readOnly": true
          },
          "isAutoClass": {
            "type": "boolean",
            "readOnly": true
          },
          "isUnicodeClass": {
            "type": "boolean",
            "readOnly": true
          },
          "isCOMObject": {
            "type": "boolean",
            "readOnly": true
          },
          "isContextful": {
            "type": "boolean",
            "readOnly": true
          },
          "isEnum": {
            "type": "boolean",
            "readOnly": true
          },
          "isMarshalByRef": {
            "type": "boolean",
            "readOnly": true
          },
          "isPrimitive": {
            "type": "boolean",
            "readOnly": true
          },
          "isValueType": {
            "type": "boolean",
            "readOnly": true
          },
          "isSignatureType": {
            "type": "boolean",
            "readOnly": true
          },
          "isSecurityCritical": {
            "type": "boolean",
            "readOnly": true
          },
          "isSecuritySafeCritical": {
            "type": "boolean",
            "readOnly": true
          },
          "isSecurityTransparent": {
            "type": "boolean",
            "readOnly": true
          },
          "structLayoutAttribute": {
            "$ref": "#/components/schemas/StructLayoutAttribute"
          },
          "typeInitializer": {
            "$ref": "#/components/schemas/ConstructorInfo"
          },
          "typeHandle": {
            "$ref": "#/components/schemas/RuntimeTypeHandle"
          },
          "guid": {
            "type": "string",
            "readOnly": true,
            "format": "uuid"
          },
          "baseType": {
            "$ref": "#/components/schemas/Type"
          },
          "isSerializable": {
            "type": "boolean",
            "readOnly": true,
            "deprecated": true
          },
          "containsGenericParameters": {
            "type": "boolean",
            "readOnly": true
          },
          "isVisible": {
            "type": "boolean",
            "readOnly": true
          }
        }
      },
      "TypeInfo": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "name": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "customAttributes": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CustomAttributeData"
            }
          },
          "isCollectible": {
            "type": "boolean",
            "readOnly": true
          },
          "metadataToken": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          },
          "isInterface": {
            "type": "boolean",
            "readOnly": true
          },
          "memberType": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              1,
              2,
              4,
              8,
              16,
              32,
              64,
              128,
              191
            ]
          },
          "namespace": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "assemblyQualifiedName": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "fullName": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "assembly": {
            "$ref": "#/components/schemas/Assembly"
          },
          "module": {
            "$ref": "#/components/schemas/Module"
          },
          "isNested": {
            "type": "boolean",
            "readOnly": true
          },
          "declaringType": {
            "$ref": "#/components/schemas/Type"
          },
          "declaringMethod": {
            "$ref": "#/components/schemas/MethodBase"
          },
          "reflectedType": {
            "$ref": "#/components/schemas/Type"
          },
          "underlyingSystemType": {
            "$ref": "#/components/schemas/Type"
          },
          "isTypeDefinition": {
            "type": "boolean",
            "readOnly": true
          },
          "isArray": {
            "type": "boolean",
            "readOnly": true
          },
          "isByRef": {
            "type": "boolean",
            "readOnly": true
          },
          "isPointer": {
            "type": "boolean",
            "readOnly": true
          },
          "isConstructedGenericType": {
            "type": "boolean",
            "readOnly": true
          },
          "isGenericParameter": {
            "type": "boolean",
            "readOnly": true
          },
          "isGenericTypeParameter": {
            "type": "boolean",
            "readOnly": true
          },
          "isGenericMethodParameter": {
            "type": "boolean",
            "readOnly": true
          },
          "isGenericType": {
            "type": "boolean",
            "readOnly": true
          },
          "isGenericTypeDefinition": {
            "type": "boolean",
            "readOnly": true
          },
          "isSZArray": {
            "type": "boolean",
            "readOnly": true
          },
          "isVariableBoundArray": {
            "type": "boolean",
            "readOnly": true
          },
          "isByRefLike": {
            "type": "boolean",
            "readOnly": true
          },
          "isFunctionPointer": {
            "type": "boolean",
            "readOnly": true
          },
          "isUnmanagedFunctionPointer": {
            "type": "boolean",
            "readOnly": true
          },
          "hasElementType": {
            "type": "boolean",
            "readOnly": true
          },
          "genericTypeArguments": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/Type"
            }
          },
          "genericParameterPosition": {
            "type": "integer",
            "readOnly": true,
            "format": "int32"
          },
          "genericParameterAttributes": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              0,
              1,
              2,
              3,
              4,
              8,
              16,
              28
            ]
          },
          "attributes": {
            "type": "integer",
            "readOnly": true,
            "format": "int32",
            "enum": [
              0,
              1,
              2,
              3,
              4,
              5,
              6,
              7,
              8,
              16,
              24,
              32,
              128,
              256,
              1024,
              2048,
              4096,
              8192,
              16384,
              65536,
              131072,
              196608,
              262144,
              264192,
              1048576,
              12582912
            ]
          },
          "isAbstract": {
            "type": "boolean",
            "readOnly": true
          },
          "isImport": {
            "type": "boolean",
            "readOnly": true
          },
          "isSealed": {
            "type": "boolean",
            "readOnly": true
          },
          "isSpecialName": {
            "type": "boolean",
            "readOnly": true
          },
          "isClass": {
            "type": "boolean",
            "readOnly": true
          },
          "isNestedAssembly": {
            "type": "boolean",
            "readOnly": true
          },
          "isNestedFamANDAssem": {
            "type": "boolean",
            "readOnly": true
          },
          "isNestedFamily": {
            "type": "boolean",
            "readOnly": true
          },
          "isNestedFamORAssem": {
            "type": "boolean",
            "readOnly": true
          },
          "isNestedPrivate": {
            "type": "boolean",
            "readOnly": true
          },
          "isNestedPublic": {
            "type": "boolean",
            "readOnly": true
          },
          "isNotPublic": {
            "type": "boolean",
            "readOnly": true
          },
          "isPublic": {
            "type": "boolean",
            "readOnly": true
          },
          "isAutoLayout": {
            "type": "boolean",
            "readOnly": true
          },
          "isExplicitLayout": {
            "type": "boolean",
            "readOnly": true
          },
          "isLayoutSequential": {
            "type": "boolean",
            "readOnly": true
          },
          "isAnsiClass": {
            "type": "boolean",
            "readOnly": true
          },
          "isAutoClass": {
            "type": "boolean",
            "readOnly": true
          },
          "isUnicodeClass": {
            "type": "boolean",
            "readOnly": true
          },
          "isCOMObject": {
            "type": "boolean",
            "readOnly": true
          },
          "isContextful": {
            "type": "boolean",
            "readOnly": true
          },
          "isEnum": {
            "type": "boolean",
            "readOnly": true
          },
          "isMarshalByRef": {
            "type": "boolean",
            "readOnly": true
          },
          "isPrimitive": {
            "type": "boolean",
            "readOnly": true
          },
          "isValueType": {
            "type": "boolean",
            "readOnly": true
          },
          "isSignatureType": {
            "type": "boolean",
            "readOnly": true
          },
          "isSecurityCritical": {
            "type": "boolean",
            "readOnly": true
          },
          "isSecuritySafeCritical": {
            "type": "boolean",
            "readOnly": true
          },
          "isSecurityTransparent": {
            "type": "boolean",
            "readOnly": true
          },
          "structLayoutAttribute": {
            "$ref": "#/components/schemas/StructLayoutAttribute"
          },
          "typeInitializer": {
            "$ref": "#/components/schemas/ConstructorInfo"
          },
          "typeHandle": {
            "$ref": "#/components/schemas/RuntimeTypeHandle"
          },
          "guid": {
            "type": "string",
            "readOnly": true,
            "format": "uuid"
          },
          "baseType": {
            "$ref": "#/components/schemas/Type"
          },
          "isSerializable": {
            "type": "boolean",
            "readOnly": true,
            "deprecated": true
          },
          "containsGenericParameters": {
            "type": "boolean",
            "readOnly": true
          },
          "isVisible": {
            "type": "boolean",
            "readOnly": true
          },
          "genericTypeParameters": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/Type"
            }
          },
          "declaredConstructors": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/ConstructorInfo"
            }
          },
          "declaredEvents": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/EventInfo"
            }
          },
          "declaredFields": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/FieldInfo"
            }
          },
          "declaredMembers": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/MemberInfo"
            }
          },
          "declaredMethods": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/MethodInfo"
            }
          },
          "declaredNestedTypes": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/TypeInfo"
            }
          },
          "declaredProperties": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/PropertyInfo"
            }
          },
          "implementedInterfaces": {
            "type": "array",
            "readOnly": true,
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/Type"
            }
          }
        }
      },
      "UpdateCategoryCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "categoryId": {
            "type": "string",
            "format": "uuid"
          },
          "name": {
            "type": "string",
            "nullable": true
          }
        }
      },
      "UpdateClientProfileCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "clientId": {
            "type": "string",
            "format": "uuid"
          },
          "companyName": {
            "type": "string",
            "nullable": true
          },
          "companyDescription": {
            "type": "string",
            "nullable": true
          },
          "companyWebsite": {
            "type": "string",
            "nullable": true
          },
          "location": {
            "type": "string",
            "nullable": true
          },
          "companyLogo": {
            "$ref": "#/components/schemas/CreateFileDto"
          },
          "employerType": {
            "type": "string",
            "nullable": true,
            "enum": [
              "Company",
              "Indivdual"
            ]
          },
          "phoneNumber": {
            "type": "string",
            "nullable": true
          }
        }
      },
      "UpdateContestCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "contestId": {
            "type": "string",
            "format": "uuid"
          },
          "title": {
            "type": "string",
            "nullable": true
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "prizePool": {
            "type": "number",
            "format": "double",
            "nullable": true
          }
        }
      },
      "UpdateFreelancerProfileCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "skills": {
            "type": "array",
            "nullable": true,
            "items": {
              "type": "string",
              "format": "uuid"
            }
          },
          "experience": {
            "type": "string",
            "nullable": true
          },
          "resume": {
            "type": "string",
            "nullable": true
          },
          "hourlyRate": {
            "type": "number",
            "format": "double",
            "nullable": true
          },
          "availability": {
            "type": "string",
            "nullable": true,
            "enum": [
              "Open",
              "Busy",
              "Vacation"
            ]
          }
        }
      },
      "UpdateFreelancerTeamCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "teamId": {
            "type": "string",
            "format": "uuid"
          },
          "name": {
            "type": "string",
            "nullable": true
          },
          "avatarFile": {
            "$ref": "#/components/schemas/CreateFileDto"
          },
          "closed": {
            "type": "boolean",
            "nullable": true
          },
          "closedReason": {
            "type": "string",
            "nullable": true
          }
        }
      },
      "UpdateJobCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "jobId": {
            "type": "string",
            "format": "uuid"
          },
          "title": {
            "type": "string",
            "nullable": true
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "requiredSkillIds": {
            "type": "array",
            "nullable": true,
            "items": {
              "type": "string",
              "format": "uuid"
            }
          },
          "categoryId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "budget": {
            "type": "number",
            "format": "double",
            "nullable": true
          },
          "hourlyRate": {
            "type": "number",
            "format": "double",
            "nullable": true
          },
          "expirationDate": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "duration": {
            "type": "integer",
            "format": "int32",
            "nullable": true
          },
          "files": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CreateFileDto"
            }
          }
        }
      },
      "UpdatePortfolioProjectCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "projectId": {
            "type": "string",
            "format": "uuid"
          },
          "name": {
            "type": "string",
            "nullable": true
          },
          "userRole": {
            "type": "string",
            "nullable": true
          },
          "skills": {
            "type": "array",
            "nullable": true,
            "items": {
              "type": "string",
              "format": "uuid"
            }
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "images": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/CreateFileDto"
            }
          },
          "hidden": {
            "type": "boolean",
            "nullable": true
          }
        }
      },
      "UpdateProposalCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "proposalId": {
            "type": "string",
            "format": "uuid"
          },
          "coverLetter": {
            "type": "string",
            "nullable": true
          },
          "proposedRate": {
            "type": "number",
            "format": "double",
            "nullable": true
          }
        }
      },
      "UpdateSkillDto": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "skillId": {
            "type": "string",
            "format": "uuid"
          },
          "name": {
            "type": "string",
            "nullable": true
          }
        }
      },
      "UpdateSkillsCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "skills": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/UpdateSkillDto"
            }
          }
        }
      },
      "UpdateTaskCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "taskId": {
            "type": "string",
            "format": "uuid"
          },
          "title": {
            "type": "string",
            "nullable": true
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "priority": {
            "type": "integer",
            "format": "int32",
            "nullable": true
          }
        }
      },
      "UpdateUserCommand": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "userId"
        ],
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "userId": {
            "type": "string",
            "format": "uuid"
          },
          "email": {
            "type": "string",
            "nullable": true
          },
          "role": {
            "type": "string",
            "nullable": true,
            "enum": [
              "User",
              "Moderator",
              "Admin",
              "Superadmin"
            ]
          },
          "avatar": {
            "$ref": "#/components/schemas/CreateFileDto"
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "name": {
            "type": "string",
            "nullable": true
          },
          "surname": {
            "type": "string",
            "nullable": true
          },
          "oldPassword": {
            "type": "string",
            "nullable": true
          },
          "newPassword": {
            "type": "string",
            "nullable": true
          },
          "telegramId": {
            "type": "string",
            "nullable": true
          }
        }
      },
      "UserEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "email",
          "fullname",
          "id",
          "isOnline",
          "name",
          "surname"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "name": {
            "type": "string",
            "minLength": 1
          },
          "surname": {
            "type": "string",
            "minLength": 1
          },
          "fullname": {
            "type": "string",
            "minLength": 1
          },
          "role": {
            "type": "string",
            "enum": [
              "User",
              "Moderator",
              "Admin",
              "Superadmin"
            ]
          },
          "type": {
            "type": "string",
            "enum": [
              "Freelancer",
              "Client",
              "NonUser"
            ]
          },
          "email": {
            "type": "string",
            "format": "email",
            "minLength": 1
          },
          "avatar": {
            "$ref": "#/components/schemas/FileEntity"
          },
          "isOnline": {
            "type": "boolean"
          },
          "isSuperadmin": {
            "type": "boolean"
          },
          "phone": {
            "type": "string",
            "format": "tel",
            "nullable": true
          },
          "telegramId": {
            "type": "string",
            "nullable": true
          },
          "blocked": {
            "type": "boolean"
          },
          "location": {
            "type": "string",
            "nullable": true,
            "enum": [
              "Kazakhstan",
              "Russia",
              "Belarus"
            ]
          },
          "isExternalUser": {
            "type": "boolean",
            "readOnly": true
          }
        }
      },
      "WalletEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "availableBalance",
          "blocked",
          "currency",
          "frozen",
          "id",
          "pendingIncome",
          "user",
          "userId"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "userId": {
            "type": "string",
            "readOnly": true,
            "format": "uuid"
          },
          "user": {
            "$ref": "#/components/schemas/UserEntity"
          },
          "currency": {
            "type": "string",
            "readOnly": true,
            "enum": [
              "RussianRuble",
              "Dollar",
              "Euro"
            ]
          },
          "frozen": {
            "$ref": "#/components/schemas/Money"
          },
          "availableBalance": {
            "$ref": "#/components/schemas/Money"
          },
          "pendingIncome": {
            "$ref": "#/components/schemas/Money"
          },
          "blocked": {
            "type": "boolean",
            "readOnly": true
          },
          "blockReason": {
            "type": "string",
            "readOnly": true,
            "enum": [
              "None",
              "FraudSuspicion",
              "LegalIssue",
              "UserRequest"
            ]
          }
        }
      },
      "WarnUserScheme": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "reason": {
            "type": "string",
            "nullable": true
          }
        }
      },
      "WithdrawProposalCommand": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "commandId": {
            "type": "string",
            "readOnly": true,
            "nullable": true
          },
          "timestamp": {
            "type": "string",
            "readOnly": true,
            "format": "date-time"
          },
          "proposalId": {
            "type": "string",
            "format": "uuid"
          }
        }
      },
      "WorkSessionEntity": {
        "type": "object",
        "additionalProperties": false,
        "required": [
          "id"
        ],
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "createdById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "createdAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "lastModifiedById": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "lastModifiedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "files": {
            "type": "array",
            "nullable": true,
            "items": {
              "$ref": "#/components/schemas/FileEntity"
            }
          },
          "contractId": {
            "type": "string",
            "format": "uuid"
          },
          "contract": {
            "$ref": "#/components/schemas/ContractEntity"
          },
          "freelancerId": {
            "type": "string",
            "format": "uuid"
          },
          "freelancer": {
            "$ref": "#/components/schemas/UserEntity"
          },
          "startDate": {
            "type": "string",
            "format": "date-time"
          },
          "endDate": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "comment": {
            "type": "string",
            "nullable": true
          },
          "clientComment": {
            "type": "string",
            "nullable": true
          },
          "submittedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "approvedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "rejectedAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "status": {
            "type": "string",
            "enum": [
              "Pending",
              "Approved",
              "Rejected"
            ]
          },
          "autoApproved": {
            "type": "boolean",
            "readOnly": true
          }
        }
      }
    },
    "securitySchemes": {
      "session": {
        "type": "apiKey",
        "description": "Session Token",
        "name": "session_token",
        "in": "cookie"
      }
    }
  },
  "security": [
    {
      "session": []
    }
  ],
  "tags": [
    {
      "name": "Auth",
      "description": "auth"
    },
    {
      "name": "UserControllers",
      "description": "auth"
    }
  ]
}