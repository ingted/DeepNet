#pragma once

#include "Utils.cuh"


struct DiagonalOneIEOP_t {
	_dev float operator() (const size_t *pos, const size_t dims) {
		if (dims == 0) {
			return 1.0;
		} else {
			bool allEqual = true;
			for (size_t dim = 1; dim <= dims; dim++) {
				if (pos[0] != pos[dim]) {
					allEqual = false;
					break;
				}
			}
			if (allEqual)
				return 1.0;
			else
				return 0.0;
		}
	}
};


template <float TValue>
struct ConstEOp_t
{
	_dev float operator() ()
	{
		return TValue;
	}
};


struct NegateEOp_t
{
	_dev float operator() (float a)
	{
		return -a;
	}
};


struct LogEOp_t
{
	_dev float operator() (float a)
	{
		return logf(a);
	}
};


struct ExpEOp_t
{
	_dev float operator() (float a)
	{
		return expf(a);
	}
};

struct IdEOp_t
{
	_dev float operator() (float a)
	{
		return a;
	}
};


struct AddEOp_t
{
	_dev float operator() (float a, float b)
	{
		return a + b;
	}
};

struct SubstractEOp_t
{
	_dev float operator() (float a, float b)
	{
		return a - b;
	}
};

struct MultiplyEOp_t
{
	_dev float operator() (float a, float b)
	{
		return a * b;
	}
};

struct DivideEOp_t
{
	_dev float operator() (float a, float b)
	{
		return a / b;
	}
};

