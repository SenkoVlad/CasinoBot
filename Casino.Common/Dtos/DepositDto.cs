﻿using Casino.BLL.Models;

namespace Casino.Common.Dtos;

public class DepositDto
{
    public DepositCurrency Currency { get; set; }
    public int AmountCents { get; set; }
}