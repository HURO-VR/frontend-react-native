def arange(start, stop=None, step=1, dtype=None):
    if stop is None:
        start, stop = 0, start
    
    if step == 0:
        raise ValueError("step must not be zero")
    
    result = []
    value = start
    
    if step > 0:
        while value < stop:
            result.append(value)
            value += step
    else:
        while value > stop:
            result.append(value)
            value += step
    
    if dtype:
        return list(map(dtype, result))
    
    return result
