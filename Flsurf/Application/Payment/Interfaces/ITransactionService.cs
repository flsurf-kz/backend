﻿using Flsurf.Application.Payment.Commands;
using Flsurf.Application.Payment.Queries;
using Flsurf.Application.Payment.UseCases;

namespace Flsurf.Application.Payment.Interfaces
{
    public interface ITransactionService
    {
        HandleTransaction HandleTransaction();
        GetTransactionsList GetTransactionsList();
        GetTransactionProviders GetTransactionProviders();
        HandleGatewayResultHandler HandleGatewayResult();
        RefundTransaction RefundTransaction(); 
        StartPaymentFlowHandler StartPaymentFlow();
        CreateSetupIntentHandler CreateSetupIntent(); 
    }
}
